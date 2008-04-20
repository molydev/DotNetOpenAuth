﻿using System;
using System.Collections.Generic;
using System.Text;
using DotNetOpenId.RelyingParty;
using System.Globalization;
using System.Diagnostics;

namespace DotNetOpenId.Extensions {
	/// <summary>
	/// The Attribute Exchange Store message, response leg.
	/// </summary>
	public class AttributeExchangeStoreResponse : IExtensionResponse {
		const string SuccessMode = "store_response_success";
		const string FailureMode = "store_response_failure";

		/// <summary>
		/// Whether the storage request succeeded.
		/// </summary>
		public bool Succeeded { get; set; }
		/// <summary>
		/// The reason for the failure.
		/// </summary>
		public string FailureReason { get; set; }

		#region IExtensionResponse Members
		string IExtension.TypeUri { get { return Constants.ae.ns; } }

		IDictionary<string, string> IExtensionResponse.Serialize(Provider.IRequest authenticationRequest) {
			var fields = new Dictionary<string, string> {
				{ "mode", Succeeded ? SuccessMode : FailureMode },
			};
			if (!Succeeded && !string.IsNullOrEmpty(FailureReason))
				fields.Add("error", FailureReason);

			return fields;
		}

		bool IExtensionResponse.Deserialize(IDictionary<string, string> fields, IAuthenticationResponse response) {
			if (fields == null) return false;
			string mode;
			if (!fields.TryGetValue("mode", out mode)) return false;
			switch (mode) {
				case SuccessMode:
					Succeeded = true;
					break;
				case FailureMode:
					Succeeded = false;
					break;
				default:
					return false;
			}

			if (!Succeeded) {
				string error;
				if (fields.TryGetValue("error", out error))
					FailureReason = error;
			}

			return true;
		}

		#endregion
	}
}
