﻿using System;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Keycloak.Net.Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Keycloak.Net
{
    public partial class KeycloakClient
    {
        private ISerializer _serializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
        });

        private readonly Url _url;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _clientSecret;
        private readonly Func<string> _getToken;

        /// <summary>
        /// With Version 17 they removed the /auth segment from the admin api
        /// </summary>
        private readonly bool _includeAuthSegment = true;

        /// <summary>
        /// Keycloak API client
        /// </summary>
        /// <param name="url">base url of keycloak</param>
        /// <param name="includeAuthSegment">if you are running a keycloak instance with version >= 17, set this to false</param>
        private KeycloakClient(string url, bool includeAuthSegment = true)
        {
            _url = url;
            _includeAuthSegment = includeAuthSegment;
        }

        /// <summary>
        /// Keycloak API client
        /// </summary>
        /// <param name="url">base url of keycloak</param>
        /// <param name="includeAuthSegment">if you are running a keycloak instance with version >= 17, set this to false</param>
        public KeycloakClient(string url, string userName, string password, bool includeAuthSegment = true)
            : this(url, includeAuthSegment)
        {
            _userName = userName;
            _password = password;
        }

        /// <summary>
        /// Keycloak API client
        /// </summary>
        /// <param name="url">base url of keycloak</param>
        /// <param name="includeAuthSegment">if you are running a keycloak instance with version >= 17, set this to false</param>
        public KeycloakClient(string url, string clientSecret, bool includeAuthSegment = true)
            : this(url, includeAuthSegment)
        {
            _clientSecret = clientSecret;
        }

        /// <summary>
        /// Keycloak API client
        /// </summary>
        /// <param name="url">base url of keycloak</param>
        /// <param name="includeAuthSegment">if you are running a keycloak instance with version >= 17, set this to false</param>
        public KeycloakClient(string url, Func<string> getToken, bool includeAuthSegment = true)
            : this(url, includeAuthSegment)
        {
            _getToken = getToken;
        }

        public void SetSerializer(ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        private IFlurlRequest GetBaseUrl(string authenticationRealm)
        {
            var url = new Url(_url);
            if (_includeAuthSegment)
                url.AppendPathSegment("/auth");

            return url.ConfigureRequest(settings => settings.JsonSerializer = _serializer)
                .WithAuthentication(_getToken, _url, authenticationRealm, _userName, _password, _clientSecret,
                    _includeAuthSegment);
        }
    }
}