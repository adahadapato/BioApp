using RestSharp;
using System;
using System.Net.Http;

namespace BioApp.ApiInfrastructure
{
    public class HttpClientInstance
    {
        private const string BaseUri = "http://www.mynecoexams.com/BiometricService/RESTWS/";
        private const string AliBaseUri = "http://5.77.43.20:8400/cafe-template/api/v1/";
        private const string PingAliSVR = "5.77.43.20";
        private const string PingJideSVR = "www.google.com";
        //http://www.mynecoexams.com/VPService/serviceoperatorapi.svc/UploadExamAttendance
        public static RestClient Instance
        {
            get { return new RestClient { BaseUrl = new Uri(BaseUri) }; }
        }

        public static HttpClient AliInstance
        {
            get { return new HttpClient { BaseAddress = new Uri(AliBaseUri) }; }
        }

        public static RestClient AliInstance2
        {
            get { return new RestClient { BaseUrl = new Uri(AliBaseUri) }; }
        }
        public static string Address
        {
            get { return BaseUri; }
        }

        public static string PingAliSVRAddress
        {
            get { return PingAliSVR; }
        }

        public static string PingJideSVRAddress
        {
            get { return PingJideSVR; }
        }
    }
}
