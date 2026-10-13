// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 商家小程序信息（<c>bizInfo</c>，列表侧与详情侧参数表<b>完全一致</b> ⇒ 共用本类型）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentBizInfo
{
    /// <summary>商家小程序的 appid（<c>appid</c>，官方全小写拼写）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商家小程序的头像（<c>headImg</c>）。</summary>
    [JsonPropertyName("headImg")]
    public string? HeadImg { get; set; }

    /// <summary>商家小程序的昵称（<c>nickName</c>）。</summary>
    [JsonPropertyName("nickName")]
    public string? NickName { get; set; }
}
