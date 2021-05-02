using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ATGCCoder {
	/// <summary>
	/// ATGC 编码器，可将各类数据编码为 ATGC 列表。
	/// </summary>
	public static class Encoder {

		private static readonly Regex spiltRegex = new(@"(\d\d)");
		private static readonly Regex matchRegex = new(@"^([01]{8})+$");


		/// <summary>
		/// 将二进制字符串编码为 ATGC 列表。
		/// </summary>
		/// <param name="bitString">要编码的二进制字符串。</param>
		/// <returns>编码结果（ATGC 列表）。</returns>
		/// <exception cref="ArgumentException">参数不是正确的二进制字符串。</exception>
		/// <exception cref="ArgumentNullException">参数为空（<see cref="null"/>）。</exception>
		/// <exception cref="EncoderFallbackException">编码过程中遇到不正确的数据。</exception>
		/// <exception cref="RegexMatchTimeoutException">正则表达式匹配超时。</exception>
		/// <exception cref="ArgumentOutOfRangeException">二进制字符串过长，无法编码。</exception>
		public static string EncodeBitString(string bitString) {
			if (matchRegex.IsMatch(bitString)) {
				StringBuilder sb = new();
				foreach (var item in spiltRegex.Replace(bitString, "$1 ").Split(' ', StringSplitOptions.RemoveEmptyEntries)) {
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
							throw new EncoderFallbackException($"Can not encode \"{item}\"!");
					}
				}
				return sb.ToString();
			} else throw new ArgumentException("Param \"bitString\" does not meet the requirements!");
		}


		/// <summary>
		/// 将字节数组编码为 ATGC 列表。
		/// </summary>
		/// <param name="data">要编码的字节数组。</param>
		/// <returns>编码结果（ATGC 列表）。</returns>
		/// <exception cref="ArgumentException">参数异常或深层堆栈异常。</exception>
		/// <exception cref="ArgumentNullException">参数为空（<see cref="null"/>）或深层堆栈异常。</exception>
		/// <exception cref="EncoderFallbackException">参数异常或深层堆栈编码过程中遇到不正确的数据。</exception>
		/// <exception cref="RegexMatchTimeoutException">深层堆栈正则表达式匹配超时。</exception>
		/// <exception cref="ArgumentOutOfRangeException">字节数组或处理后的数据过大，无法编码。</exception>
		public static string Encode(byte[] data) {
			StringBuilder sb = new();
			foreach (var one in data) {
				var temp = Convert.ToString(one, 2);
				sb.Append(temp.Insert(0, new string('0', 8 - temp.Length)));
			}
			return EncodeBitString(sb.ToString());
		}


		/// <summary>
		/// 将字符串解码后编码为 ATGC 列表。
		/// </summary>
		/// <param name="value">要编码的字符串。</param>
		/// <param name="encoding">解码字符串使用的编码类型。</param>
		/// <returns>编码结果（ATGC 列表）。</returns>
		/// <exception cref="ArgumentException">参数异常或深层堆栈异常。</exception>
		/// <exception cref="ArgumentNullException">参数为空（<see cref="null"/>）或深层堆栈异常。</exception>
		/// <exception cref="EncoderFallbackException">字符串解码异常或深层堆栈编码过程中遇到不正确的数据。</exception>
		/// <exception cref="RegexMatchTimeoutException">深层堆栈正则表达式匹配超时。</exception>
		/// <exception cref="ArgumentOutOfRangeException">字符串过长或处理后的数据过大，无法编码。</exception>
		public static string EncodeString(string value, Encoding encoding) {
			var data = encoding.GetBytes(value);
			return Encode(data);
		}


		/// <summary>
		/// 将字符串使用 UTF-8 编码解码后编码为 ATGC 列表。
		/// </summary>
		/// <param name="value">要编码的字符串。</param>
		/// <returns>编码结果（ATGC 列表）。</returns>
		/// <exception cref="ArgumentException">参数异常或深层堆栈异常。</exception>
		/// <exception cref="ArgumentNullException">参数为空（<see cref="null"/>）或深层堆栈异常。</exception>
		/// <exception cref="EncoderFallbackException">字符串解码异常或深层堆栈编码过程中遇到不正确的数据。</exception>
		/// <exception cref="RegexMatchTimeoutException">深层堆栈正则表达式匹配超时。</exception>
		/// <exception cref="ArgumentOutOfRangeException">字符串过长或处理后的数据过大，无法编码。</exception>
		public static string EncodeString(string value) {
			return EncodeString(value, Encoding.UTF8);
		}


		/// <summary>
		/// 将流编码为 ATGC 列表。
		/// </summary>
		/// <param name="stream">要编码的流。</param>
		/// <returns>编码结果（ATGC 列表）。</returns>
		/// <exception cref="ArgumentException">参数异常或深层堆栈异常。</exception>
		/// <exception cref="ArgumentNullException">深层堆栈异常。</exception>
		/// <exception cref="EncoderFallbackException">深层堆栈编码过程中遇到不正确的数据。</exception>
		/// <exception cref="RegexMatchTimeoutException">深层堆栈正则表达式匹配超时。</exception>
		/// <exception cref="ArgumentOutOfRangeException">流过长或处理后的数据过大，无法编码。</exception>
		/// <exception cref="IOException">输入/输出异常。</exception>
		/// <exception cref="NotSupportedException">流不支持读取、获取位置、切换位置等操作。</exception>
		/// <exception cref="ObjectDisposedException">流已被关闭或释放。</exception>
		public static string EncodeStream(Stream stream) {
			if (stream.CanRead && stream.CanSeek) {
				var position = stream.Position;
				stream.Position = 0;
				var data = new byte[stream.Length];
				stream.Read(data, 0, data.Length);
				stream.Position = position;
				return Encode(data);
			} else throw new NotSupportedException("Stream can not read or seek.");
		}


		/// <summary>
		/// 将文件流编码为 ATGC 列表。<br/>
		/// 功能与 <see cref="EncodeStream(Stream)"/> 基本一致，但仅支持编码文件流。
		/// </summary>
		/// <param name="fileStream">要编码的文件流。</param>
		/// <returns>编码结果（ATGC 列表）。</returns>
		/// <exception cref="ArgumentException">参数异常或深层堆栈异常。</exception>
		/// <exception cref="ArgumentNullException">深层堆栈异常。</exception>
		/// <exception cref="EncoderFallbackException">深层堆栈编码过程中遇到不正确的数据。</exception>
		/// <exception cref="RegexMatchTimeoutException">深层堆栈正则表达式匹配超时。</exception>
		/// <exception cref="ArgumentOutOfRangeException">流过长或处理后的数据过大，无法编码。</exception>
		/// <exception cref="IOException">输入/输出异常。</exception>
		/// <exception cref="NotSupportedException">流不支持读取、获取位置、切换位置等操作。</exception>
		/// <exception cref="ObjectDisposedException">流已被关闭或释放。</exception>
		public static string EncodeFile(FileStream fileStream) {
			return EncodeStream(fileStream);
		}


		/// <summary>
		/// 将文件编码为 ATGC 列表。
		/// </summary>
		/// <param name="filePath">要编码的文件路径。</param>
		/// <returns>编码结果（ATGC 列表）。</returns>
		/// <exception cref="ArgumentException">参数异常或深层堆栈异常。</exception>
		/// <exception cref="ArgumentNullException">参数异常或深层堆栈异常。</exception>
		/// <exception cref="EncoderFallbackException">深层堆栈编码过程中遇到不正确的数据。</exception>
		/// <exception cref="RegexMatchTimeoutException">深层堆栈正则表达式匹配超时。</exception>
		/// <exception cref="ArgumentOutOfRangeException">文件或处理后的数据过大，无法编码。</exception>
		/// <exception cref="IOException">输入/输出异常。</exception>
		/// <exception cref="NotSupportedException">文件流不支持读取、获取位置、切换位置等操作。</exception>
		/// <exception cref="PathTooLongException">文件路径过长。</exception>
		/// <exception cref="DirectoryNotFoundException">文件夹不存在。</exception>
		/// <exception cref="FileNotFoundException">文件不存在。</exception>
		/// <exception cref="UnauthorizedAccessException">输入/输出异常或发生特定类型安全错误。</exception>
		/// <exception cref="System.Security.SecurityException">发生安全错误。</exception>
		public static string EncodeFile(string filePath) {
			return Encode(File.ReadAllBytes(filePath));
		}


		/// <summary>
		/// 将文件编码为 ATGC 列表。
		/// </summary>
		/// <param name="fileInfo">要编码的文件的文件信息实例。</param>
		/// <returns>编码结果（ATGC 列表）。</returns>
		/// <exception cref="ArgumentException">参数异常或深层堆栈异常。</exception>
		/// <exception cref="ArgumentNullException">参数异常或深层堆栈异常。</exception>
		/// <exception cref="EncoderFallbackException">深层堆栈编码过程中遇到不正确的数据。</exception>
		/// <exception cref="RegexMatchTimeoutException">深层堆栈正则表达式匹配超时。</exception>
		/// <exception cref="ArgumentOutOfRangeException">文件或处理后的数据过大，无法编码。</exception>
		/// <exception cref="IOException">输入/输出异常。</exception>
		/// <exception cref="NotSupportedException">文件流不支持读取、获取位置、切换位置等操作。</exception>
		/// <exception cref="PathTooLongException">文件路径过长。</exception>
		/// <exception cref="DirectoryNotFoundException">文件夹不存在。</exception>
		/// <exception cref="FileNotFoundException">文件不存在。</exception>
		/// <exception cref="UnauthorizedAccessException">输入/输出异常或发生特定类型安全错误。</exception>
		/// <exception cref="System.Security.SecurityException">发生安全错误。</exception>
		public static string EncodeFile(FileInfo fileInfo) {
			return EncodeFile(fileInfo.FullName);
		}
	}
}
