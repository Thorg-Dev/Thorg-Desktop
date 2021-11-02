﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GolemUI.Miners.Claymore;
using Microsoft.Extensions.Logging;

namespace GolemUI.Miners.Phoenix
{
    public class PhoenixMiner : IMinerApp
    {
        private MinerAppName _minerAppName;
        public MinerAppName MinerAppName => _minerAppName;

        private ILogger _logger;
        public PhoenixMiner(ILogger<PhoenixMiner> logger)
        {
            _logger = logger;
            _minerAppName = new MinerAppName(MinerAppName.MinerAppEnum.Phoenix);
        }

        public string WorkingDir => @"plugins\claymore";

        public string ExePath => @"plugins\claymore\EthDcrMiner64.exe";

        public string PreBenchmarkParams => "-epool test -li 200";

        public string GetBenchmarkParams(string pool, string ethereumAddress, string nodeName, string cards, string niceness)
        {
            string extraParams = "";
            if (!string.IsNullOrEmpty(cards))
            {
                extraParams += " -gpus " + cards;
            }
            if (!string.IsNullOrEmpty(niceness))
            {
                extraParams += " -li " + niceness;
            }

            return $"-wd 0 -r -1 -epool {pool} -ewal {ethereumAddress} -eworker \"benchmark:0x0/{nodeName}:{ethereumAddress}/0\" -clnew 1 -clKernel 0" + extraParams;
        }

        public IMinerParser CreateParserForBenchmark()
        {
            return new ClaymoreParser(false, 5, _logger);
        }

        public IMinerParser CreateParserForPreBenchmark()
        {
            return new ClaymoreParser(true, 5, _logger);
        }
    }
}