using GolemUI.Command;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static GolemUI.Interfaces.IPaymentService;

namespace GolemUI.Src.EIP712
{
    public class GasslessForwarderConfig
    {
        public String RpcUrl;
        public Network NetworkName;
        public String ForwarderUrl;

        public GasslessForwarderConfig(string rpcUrl, Network networkName, String forwarderUrl)
        {
            RpcUrl = rpcUrl;
            NetworkName = networkName;
            ForwarderUrl = forwarderUrl;
        }
    }

    public class Eip712Request
    {
        public byte[]? Message;
        public byte[]? FunctionCallEncodedInAbi;
        public byte[]? SignedMessage;

        public string? R;
        public string? S;
        public string? V;

        public string? SenderAddress;
    }

    class GaslessForwarderException : Exception
    {
        public GaslessForwarderException(string message, Exception? cause = null) : base(message, cause)
        {
        }
    }

    public class GasslessForwarderService
    {
        GasslessForwarderConfig _config;
        public GasslessForwarderService(GasslessForwarderConfig config)
        {
            _config = config;
        }


        public async Task<(string BlockHash, BigInteger Amount)> SnapState(string networkName, string fromAddress)
        {
            var contractAddress = GolemContractAddress.Get(networkName);
            var contract = new GolemContract(_config.RpcUrl, contractAddress);

            return await contract.SnapState(fromAddress);
        }

        public async Task<Eip712Request> GetEip712EncodedTransferRequest(string networkName, string fromAddress, string recipentAddress, BigInteger amount, BlockParameter blockParameter = null)
        {
            var contractAddress = GolemContractAddress.Get(networkName);
            GolemContract contract = new GolemContract(_config.RpcUrl, contractAddress);
            BigInteger nonce = await contract.GetNonce(fromAddress);
            var functionEncodedInAbi = contract.GetTransferFunctionAbi(recipentAddress, amount);
            var payload = EIP712MetaTransactionPayload.GenerateForTrasfer(networkName, contractAddress, fromAddress, nonce, functionEncodedInAbi);

            return new Eip712Request()
            {
                FunctionCallEncodedInAbi = functionEncodedInAbi,
                Message = payload
            };
        }

        public async Task<string> SendRequest(Eip712Request request)
        {
            HttpClient httpClient = new HttpClient();
            var payload = new { r = request.R, v = request.V, s = request.S, sender = request.SenderAddress, signedRequest = "0x" + request.SignedMessage.ToHex(), abiFunctionCall = "0x" + request.FunctionCallEncodedInAbi.ToHex() };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var result = await httpClient.PostAsync(_config.ForwarderUrl, content);
            var resultBody = await result.Content.ReadAsStringAsync();
            Console.WriteLine("SERVER : " + result.StatusCode + " : " + resultBody);

            dynamic data = JsonConvert.DeserializeObject<dynamic>(resultBody);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (data["txId"] != null)
                {
                    return data.txId.ToString();
                }
                else
                {
                    throw new GaslessForwarderException("No txhash value from the forwarder");
                }
            }
            else if ((int)result.StatusCode == 429)
            {
                // Grace period failure
                throw new GaslessForwarderException(data["message"].ToString() + "\nRetry after: " + result.Headers.RetryAfter.ToString());
            }
            else
            {
                throw new GaslessForwarderException(data["message"].ToString());
            }
        }
    }
}
