// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Auth;

/// <summary>检查加密信息应答（<c>POST /wxa/business/checkencryptedmsg</c>）。</summary>
/// <remarks>
/// <b>官方字段名照录</b>：返回字段是 <c>vaild</c>（官方原文拼写，非 <c>valid</c>），
/// 取值 <c>true</c> = 微信生成、<c>false</c> = 非微信生成或已超 3 天。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaCheckEncryptedMsgResponse : WxaResponse
{
    /// <summary>是否由微信生成（<c>vaild</c>，官方原文拼写）。</summary>
    [JsonPropertyName("vaild")]
    public bool? Vaild { get; set; }
}
