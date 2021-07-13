﻿using GolemUI.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GolemUI.Claymore
{

    public class ClaymoreGpuStatus : ICloneable
    {
        public int gpuNo { get; set; }
        public string? gpuName { get; set; }
        public int? PciExpressLane { get; set; }
        public bool OutOfMemory { get; set; }
        public bool GPUNotFound { get; set; }
        public float BenchmarkSpeed { get; set; }
        public bool IsDagCreating { get; set; }
        public bool IsEnabledByUser { get; set; }
        public float DagProgress { get; set; }
        public string? GPUVendor { get; set; }
        public string? GPUDetails { get; set; }
        public string? GPUError { get; set; }

        public ClaymoreGpuStatus(int gpuNo)
        {
            IsEnabledByUser = true;
            this.gpuNo = gpuNo;
        }

        public object Clone()
        {
            ClaymoreGpuStatus s = new ClaymoreGpuStatus(this.gpuNo);
            s.gpuName = this.gpuName;
            s.OutOfMemory = this.OutOfMemory;
            s.GPUNotFound = this.GPUNotFound;
            s.BenchmarkSpeed = this.BenchmarkSpeed;
            s.IsDagCreating = this.IsDagCreating;
            s.DagProgress = this.DagProgress;
            s.GPUVendor = this.GPUVendor;
            s.GPUDetails = this.GPUDetails;
            s.PciExpressLane = this.PciExpressLane;
            s.IsEnabledByUser = this.IsEnabledByUser;
            s.GPUError = this.GPUError;
            return s;
        }

        public bool IsReadyForMining()
        {
            if (IsDagFinished() && BenchmarkSpeed > 0.5 && String.IsNullOrEmpty(GPUError))
            {
                return true;
            }
            return false;
        }

        public bool IsDagFinished()
        {
            if (DagProgress < 1.0f)
            {
                return false;
            }
            if (IsDagCreating)
            {
                return false;
            }
            return true;
        }
        public bool IsOperationStopped()
        {
            if (OutOfMemory || GPUNotFound)
            {
                return true;
            }
            return false;
        }
    }

    public class ClaymoreLiveStatus : ICloneable
    {
        public string BenchmarkError = "";

        Dictionary<int, ClaymoreGpuStatus> _gpus = new Dictionary<int, ClaymoreGpuStatus>();
        public Dictionary<int, ClaymoreGpuStatus> GPUs { get { return _gpus; } }

        public bool BenchmarkFinished = false;

        public float BenchmarkTotalSpeed { get; set; }
        public string? ErrorMsg = null;
        public bool GPUInfosParsed { get; set; }
        public int NumberOfClaymorePerfReports { get; set; }
        public int TotalClaymoreReportsBenchmark { get; set; }

        public bool IsBenchmark;

        public List<int> GetEnabledGpus()
        {
            List<int> result = new List<int>();
            foreach (KeyValuePair<int, ClaymoreGpuStatus> entry in this._gpus)
            {
                ClaymoreGpuStatus st = entry.Value;
                if (st.BenchmarkSpeed > 0.1 && st.IsDagFinished() && st.IsEnabledByUser)
                {
                    result.Add(st.gpuNo);
                }
            }
            return result;
        }

        public ClaymoreLiveStatus(bool isBenchmark)
        {
            IsBenchmark = isBenchmark;
            TotalClaymoreReportsBenchmark = 5;
        }

        public object Clone()
        {
            ClaymoreLiveStatus s = new ClaymoreLiveStatus(this.IsBenchmark);
            s.BenchmarkFinished = this.BenchmarkFinished;
            s.BenchmarkTotalSpeed = this.BenchmarkTotalSpeed;
            //s.BenchmarkProgress = this.BenchmarkProgress;
            s.ErrorMsg = this.ErrorMsg;
            s.GPUInfosParsed = this.GPUInfosParsed;
            s.NumberOfClaymorePerfReports = this.NumberOfClaymorePerfReports;
            s.TotalClaymoreReportsBenchmark = this.TotalClaymoreReportsBenchmark;

            s._gpus = new Dictionary<int, ClaymoreGpuStatus>();
            foreach (KeyValuePair<int, ClaymoreGpuStatus> entry in this._gpus)
            {
                s._gpus.Add(entry.Key, (ClaymoreGpuStatus) entry.Value.Clone());
            }

            return s;
        }

        public bool AreAllDagsFinishedOrFailed()
        {
            foreach (var gpu in GPUs.Values)
            {
                bool gpuDagFinished = gpu.IsDagFinished() || gpu.IsOperationStopped();
                if (!gpuDagFinished)
                {
                    return false;
                }
            }
            return true;
        }

        public float GetEstimatedBenchmarkProgress()
        {
            const float INFO_PARSED = 0.1f;
            const float DAG_CREATION_STOP = 0.5f;

            if (!GPUInfosParsed)
            {
                return 0.0f;
            }
            if (!AreAllDagsFinishedOrFailed())
            {
                float minDagProgress = 1.0f;
                foreach (var gpu in GPUs.Values)
                {
                    if (gpu.DagProgress < minDagProgress)
                    {
                        minDagProgress = gpu.DagProgress;
                    }
                }
                return INFO_PARSED + minDagProgress * (DAG_CREATION_STOP - INFO_PARSED);
            }
            return DAG_CREATION_STOP + (float)NumberOfClaymorePerfReports / TotalClaymoreReportsBenchmark * (1.0f - DAG_CREATION_STOP);



        }
    }

    public class ClaymoreBenchmarkLine
    {
        public long delta_time_ms { get; set; }
        public string line = "";
    }

    public class ClaymoreParser
    {
        const StringComparison STR_COMP_TYPE = StringComparison.InvariantCultureIgnoreCase;

        ClaymoreLiveStatus _liveStatus;

        private readonly object __lockObj = new object();

        private bool _readyForGpusEthInfo = false;

        private DateTime _start;
        private string? _benchmarkRecordingPath = null;


        private bool _gpusInfosParsed = false;
        public bool AreAllCardInfosParsed()
        {
            return _gpusInfosParsed;
        }

        public ClaymoreParser(bool isBenchmark)
        {
            _liveStatus = new ClaymoreLiveStatus(isBenchmark);
        }

        /// <summary>
        /// Thread safe 
        /// </summary>
        /// <returns>copy of ClaymoreLiveStatus structure</returns>
        public ClaymoreLiveStatus GetLiveStatusCopy()
        {
            lock (__lockObj)
            {
                return (ClaymoreLiveStatus)_liveStatus.Clone();
                // Your code...
            }
        }

        /// <summary>
        /// call before parsing to get good recording
        /// </summary>
        public void BeforeParsing()
        {
            _start = DateTime.Now;
            string benchmarkRecordingFolder = SettingsLoader.GetLocalPath();
            string benchmarkRecordingFile = "Benchmark_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".recording";
            _benchmarkRecordingPath = Path.Combine(benchmarkRecordingFolder, benchmarkRecordingFile);
            StreamWriter? sw = null;
            try
            {
                sw = new StreamWriter(_benchmarkRecordingPath, false);
                sw.WriteLine("0: Start recording");
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// Parse output line of claymore process
        /// </summary>
        public void ParseLine(string line)
        {
            lock (__lockObj)
            {
                // Your code...

#if DEBUG
                ClaymoreBenchmarkLine benchLine = new ClaymoreBenchmarkLine();
                benchLine.line = line;
                benchLine.delta_time_ms = (long)(DateTime.Now - _start).TotalMilliseconds;
                Debug.WriteLine(String.Format("{0}: {1}", benchLine.delta_time_ms, line));
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(_benchmarkRecordingPath, true);
                    sw.WriteLine(String.Format("{0}: {1}", benchLine.delta_time_ms, line));
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Close();
                    }
                }
#endif
                string lineText = line;
                //output contains spelling error avaiable instead of available, checking for boths:
                if (lineText == null)
                    return;

                int gpuNo = -1;
                string gpu_claymore_index = "";
                ClaymoreGpuStatus? currentStatus = null;

                int indexNo = 0;
                foreach (var key in new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G" })
                {
                    indexNo += 1;
                    if (lineText.StartsWith($"GPU{key}"))
                    {
                        gpuNo = indexNo;
                        gpu_claymore_index = key;
                        break;
                    }
                }

                if (lineText == "Fatal error detected")
                {
                    _liveStatus.ErrorMsg = lineText;
                }

                if (gpuNo != -1)
                {
                    if (!_liveStatus.GPUs.ContainsKey(gpuNo))
                    {
                        _liveStatus.GPUs.Add(gpuNo, new ClaymoreGpuStatus(gpuNo));
                    }
                    currentStatus = _liveStatus.GPUs[gpuNo];


                    if (currentStatus.GPUVendor == null)
                    {
                        bool nVidiaGpuFound = false;
                        bool amdGpuFound = false;

                        if (lineText.Contains("NVIDIA", STR_COMP_TYPE) ||
                            lineText.Contains("GeForce", STR_COMP_TYPE) ||
                            lineText.Contains("Quadro", STR_COMP_TYPE) ||
                            lineText.Contains("CUDA", STR_COMP_TYPE)
                            )
                        {
                            currentStatus.GPUVendor = "nVidia";
                            nVidiaGpuFound = true;
                        }

                        if (lineText.Contains("RADEON", STR_COMP_TYPE))
                        {
                            currentStatus.GPUVendor = "AMD";
                            amdGpuFound = true;
                        }

                        if ((nVidiaGpuFound || amdGpuFound) && lineText.Contains(":"))
                        {
                            //todo - what happens when details contains :
                            currentStatus.GPUDetails = lineText.Split(":")[1].Trim();

                            if (currentStatus.GPUDetails.Contains(','))
                            {
                                currentStatus.gpuName = currentStatus.GPUDetails.Split(',')[0];
                                if (currentStatus.gpuName.Contains("(pcie"))
                                {
                                    var split2 = currentStatus.gpuName.Split("(pcie");
                                    currentStatus.gpuName = split2[0].Trim();
                                    int pciExpressLane;
                                    if (split2.Length >= 2)
                                    {
                                        bool success = int.TryParse(split2[1].Trim(new char[] { ' ', ')' }), out pciExpressLane);
                                        if (success)
                                        {
                                            currentStatus.PciExpressLane = pciExpressLane;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (lineText.Contains(": clSetKernelArg"))
                    {
                        currentStatus.GPUError = lineText;
                    }

                    if (lineText.Contains(": Starting up", STR_COMP_TYPE))
                    {
                        _liveStatus.GPUInfosParsed = true;
                    }

                    if (lineText.Contains(": Allocating DAG", STR_COMP_TYPE))
                    {
                        _liveStatus.GPUInfosParsed = true;
                        currentStatus.IsDagCreating = true;
                        currentStatus.DagProgress = 0.0f;
                    }
                    if (lineText.Contains(": Generating DAG", STR_COMP_TYPE))
                    {
                        _liveStatus.GPUInfosParsed = true;
                        currentStatus.IsDagCreating = true;
                        currentStatus.DagProgress = 0.05f;
                    }

                    if (lineText.Contains(": DAG", STR_COMP_TYPE))
                    {
                        var splits = lineText.Split(" ");

                        foreach (var split in splits)
                        {
                            if (split.Contains("%"))
                            {
                                string percentDag = split.Trim(new char[] { ' ', '%' });
                                double res = 0;
                                if (double.TryParse(percentDag, NumberStyles.Float, CultureInfo.InvariantCulture, out res))
                                {
                                    currentStatus.DagProgress = (float)(res / 100.0);
                                }
                                else
                                {
                                    //handle error somehow??
                                }
                            }
                        }
                    }

                    if (lineText.Contains(": DAG generated", STR_COMP_TYPE))
                    {
                        currentStatus.IsDagCreating = false;
                        currentStatus.DagProgress = 1.0f;
                    }
                    if (lineText.Contains("out of memory", STR_COMP_TYPE))
                    {
                        currentStatus.GPUError = lineText;
                    }

                }
                if (lineText.StartsWith("Eth speed", STR_COMP_TYPE))
                {
                    _readyForGpusEthInfo = true;

                    var splits = lineText.Split(" ");
                    double val;
                    if (splits.Length > 2 && double.TryParse(splits[2], NumberStyles.Float, CultureInfo.InvariantCulture, out val))
                    {
                        double s = val;

                        _liveStatus.BenchmarkTotalSpeed = (float)val;

                        if (_liveStatus.BenchmarkTotalSpeed > 0.1 && _liveStatus.GPUs.Count == 1)
                        {
                            _liveStatus.GPUs.First().Value.BenchmarkSpeed = _liveStatus.BenchmarkTotalSpeed;
                            _liveStatus.NumberOfClaymorePerfReports += 1;
                        }
                    }
                }

                if (_readyForGpusEthInfo && lineText.StartsWith("GPU", STR_COMP_TYPE))
                {
                    //sample:
                    //"GPUs: 1: 0.000 MH/s (0) 2: 0.000 MH/s (0)"

                    var splits = lineText.Split("MH/s");


                    for (int i = 0; i < splits.Length - 1; i++)
                    {
                        //double val;
                        //if (split.Length > 2 && double.TryParse(split[2], out val))
                        var s = splits[i].TrimEnd().Split(" ");

                        var p = s.Last();
                        double mhs;

                        bool parsedOK = double.TryParse(p, NumberStyles.Float, CultureInfo.InvariantCulture, out mhs);

                        if (parsedOK)
                        {
                            int parsedGpuNo = i + 1;

                            if (!_liveStatus.GPUs.ContainsKey(parsedGpuNo))
                            {
                                _liveStatus.GPUs.Add(parsedGpuNo, new ClaymoreGpuStatus(parsedGpuNo));
                            }
                            _liveStatus.GPUs[parsedGpuNo].BenchmarkSpeed = (float)mhs;
                        }


                    }

                    if (_liveStatus.AreAllDagsFinishedOrFailed())
                    {
                        _liveStatus.NumberOfClaymorePerfReports += 1;
                    }
                }
            }
        }
    }

}
