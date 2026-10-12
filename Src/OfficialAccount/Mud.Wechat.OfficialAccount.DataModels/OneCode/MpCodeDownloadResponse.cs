// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OneCode;

/// <summary>
/// 下载二维码包（<c>POST /intp/marketcode/applycodedownload</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档缺陷（照录，SDK 的建模裁决）</b>：官方「返回参数」表把 <c>buffer</c> 的类型列为
/// 「<b>formdata 文件 buffer</b>」（<b>非合法 JSON 类型，无法据以建模</b>），而同页返回示例为
/// <b>JSON 包裹</b>的 <c>{"errcode":…,"errmsg":…,"buffer":"…"}</c>。SDK 按<b>示例</b>建模为
/// <see cref="Buffer"/> 字符串（base64），符合「表类型列无法建模 ⇒ 取示例」的处置。
/// </para>
/// <para>
/// <b>解码链（官方原文，SDK 不代解码）</b>：「需先 <b>base64 decode</b>，再做<b>解密</b>操作」；
/// 官方注「解密参见 3.1」——<b>该章节在本页不存在</b>（章节跳至 4/5 节），解密算法与密钥来源官方未给出
/// ⇒ SDK <b>不实现</b>解码/解密，由宿主按官方专项文档处理。
/// </para>
/// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpCodeDownloadResponse : MpResponse
{
    /// <summary>获取或设置二维码包数据（官方 <c>buffer</c>；base64 编码，须 decode 后解密；SDK 不代解码）。</summary>
    [JsonPropertyName("buffer")]
    public string? Buffer { get; set; }
}
