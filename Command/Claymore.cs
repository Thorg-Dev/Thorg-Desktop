﻿using GolemUI.Claymore;
using GolemUI.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GolemUI.Command
{


    public class ClaymoreBenchmark
    {

        private static Mutex mut = new Mutex();

        public LogLineHandler? LineHandler { get; set; }

        string _claymore_working_dir = @"plugins\claymore";
        string _claymore_exe_path = @"plugins\claymore\EthDcrMiner64.exe";

        public string BenchmarkError = "";

        public volatile bool GPUNotFound;
        public volatile bool OutOfMemory;
        public volatile bool BenchmarkFinished;
        public volatile float BenchmarkProgress;
        public volatile float BenchmarkSpeed;

        private object _sync = new object();

        public string? GPUVendor { get; set; }

        private string? _unsafeGpuDetails;

        //private ClaymoreLiveStatus _liveStatus = new ClaymoreLiveStatus();
        private ClaymoreParser _claymoreParser = new ClaymoreParser(isBenchmark:true);
        public ClaymoreParser ClaymoreParser { get { return _claymoreParser; } }

        public string? GPUDetails
        {
            get {
                lock (_sync)
                {
                    //return copy to be more thread safe
                    if (_unsafeGpuDetails == null)
                    {
                        return null;
                    }
                    return new String(_unsafeGpuDetails);
                }
            }
            set
            {
                lock (_sync)
                {
                    _unsafeGpuDetails = value;
                }
            }
        }



        Process? _claymoreProcess;

        int? _gpuNo;
        public ClaymoreBenchmark(int? gpuNo)
        {
            _gpuNo = gpuNo;

        }

        public void Stop()
        {
            if (_claymoreProcess != null)
            {
                _claymoreProcess.Kill(entireProcessTree: true);
                _claymoreProcess = null;
            }
        }

        public bool RunBenchmark(string cards)
        {
            BenchmarkError = "";
            BenchmarkFinished = false;
            GPUNotFound = false;

            this.BenchmarkProgress = 0.0f;

            var startInfo = new ProcessStartInfo
            {
                FileName = this._claymore_exe_path,
                WorkingDirectory = this._claymore_working_dir,
                //Arguments = null,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            

            List<string> arguments = new List<string>();

            //Enable benchmark mode:

            arguments.AddRange("-epool staging-backend.chessongolem.app:3334 -ewal 0xD593411F3E6e79995E787b5f81D10e12fA6eCF04 -eworker benchmark".Split(" "));

            if (!string.IsNullOrEmpty(cards))
            {
                arguments.Add("-di");
                arguments.Add(cards);
            }
            //Set GPU number to test:
            if (this._gpuNo != null && this._gpuNo.ToString() != null)
            {
                string? s = this._gpuNo.ToString();
                if (s != null)
                {
                    arguments.AddRange(new string[] { "-di", s});
                }
            }

            foreach (var arg in arguments)
            {
                if (arg == null)
                {
                    throw new ArgumentNullException();
                }
                startInfo.ArgumentList.Add(arg);
            }

            _claymoreProcess = new Process
            {
                StartInfo = startInfo
            };

            try
            {
                _claymoreProcess.Start();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                BenchmarkError = $"Process {this._claymore_exe_path} cannot be run, check antivirus settings";
                return false;
            }
            _claymoreParser.BeforeParsing();
            _claymoreProcess.OutputDataReceived += OnOutputDataRecv;
            _claymoreProcess.ErrorDataReceived += OnErrorDataRecv;
            _claymoreProcess.BeginErrorReadLine();
            _claymoreProcess.BeginOutputReadLine();

            /*
            var t = new Thread(() =>
            {
                while (_claymoreProcess.WaitForExit(300))
                {
                    //do stuff waiting for claymore process
                }
                this.BenchmarkFinished = true;
            });*/
            this.BenchmarkProgress = 0.1f;
            //t.Start();
            return true;
        }

        void OnOutputDataRecv(object sender, DataReceivedEventArgs e)
        {
            string? lineText = e.Data;
            //output contains spelling error avaiable instead of available, checking for boths:
            if (lineText == null)
                return;

            _claymoreParser.ParseLine(lineText);

        }

        void OnErrorDataRecv(object sender, DataReceivedEventArgs e)
        {
            if (LineHandler != null && e.Data != null)
            {
                LineHandler("claymore", e.Data);
            }
        }


    }
}
