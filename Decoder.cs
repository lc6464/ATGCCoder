using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ATGCCoder {
	public class Decoder {
		private static readonly Regex spiltRegex = new(@"^(\d\d)$");
		private static readonly Regex matchRegex = new(@"^([01]{8})+$");
		public static string EncodeBitString(string bitString) {
			if (matchRegex.IsMatch(bitString)) {
				StringBuilder sb = new();
				foreach (var item in spiltRegex.Replace(bitString, " $1")[1..].Split(' ')) {
					switch (item) {
						case "00":
							sb.Append('A');
							break;
						case "01":
							sb.Append('T');
							break;
						case "10":
							sb.Append('G');
							break;
						case "11":
							sb.Append('C');
							break;
						default:
							return $"Error: Can not encode \"{item}\" to ATGC-List!";
					}
				}
				return sb.ToString();
			}
			return $"Error: Param \"bitString\" soes not meet the requirements!";
		}
	}
}
