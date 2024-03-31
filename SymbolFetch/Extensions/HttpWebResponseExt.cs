using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xaml;

namespace SymbolFetch
{
    public class RequestState
    {
        // This class stores the State of the request.
        const int BUFFER_SIZE = 1024;
        public StringBuilder requestData;
        public byte[] BufferRead;
        public HttpWebRequest request;
        public HttpWebResponse response;
        public Stream streamResponse;
        public RequestState()
        {
            BufferRead = new byte[BUFFER_SIZE];
            requestData = new StringBuilder("");
            request = null;
            streamResponse = null;
        }
    }
    public static class HttpWebResponseExt
    {
        public static HttpWebResponse GetResponseNoException(this HttpWebRequest req)
        {
            try
            {
                return (HttpWebResponse)req.GetResponse();
            }
            catch (WebException we)
            {
                var resp = we.Response as HttpWebResponse;
                if (resp == null)
                    throw;
                return resp;
            }
        }
        /// <summary>
        /// todo   support async
        /// </summary>
        /// <param name="req"></param>
        /// <param name="callback"></param>
        /// <param name="requestState"></param>
        /// <returns></returns>

        public static HttpWebResponse GetResponseNoExceptionAsync(this HttpWebRequest req, AsyncCallback callback, RequestState requestState)
        {
            try
            {
                IAsyncResult result = (IAsyncResult)req.BeginGetResponse(callback,null);

                return (HttpWebResponse)req.GetResponse();
            }
            catch (WebException we)
            {
                var resp = we.Response as HttpWebResponse;
                if (resp == null)
                    throw;
                return resp;
            }
        }
        public static void RespCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                // State of request is asynchronous.
                RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
                HttpWebRequest myHttpWebRequest = myRequestState.request;
                myRequestState.response = (HttpWebResponse)myHttpWebRequest.EndGetResponse(asynchronousResult);

                // Read the response into a Stream object.
                Stream responseStream = myRequestState.response.GetResponseStream();
                myRequestState.streamResponse = responseStream;

                // Begin the Reading of the contents of the HTML page and print it to the console.
                //IAsyncResult asynchronousInputRead = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                return;
            }
            catch (WebException e)
            {
                Console.WriteLine("\nRespCallback Exception raised!");
                Console.WriteLine("\nMessage:{0}", e.Message);
                Console.WriteLine("\nStatus:{0}", e.Status);
            }
        }
    }
}
