// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopSharer;

/// <summary>
/// 已绑定分享员列表应答（<c>POST /shop/sharer/get_sharer_list</c>）。
/// </summary>
/// <remarks>
/// <para><b>列表键名为 <c>sharers</c></b>（官方原文），不是 <c>list</c> / <c>data.list</c>；
/// 本域三支列表的列表键名各不同（<c>sharers</c> / <c>orders</c> / <c>lives</c>）⇒ 守卫 SHOPSH5 逐条锁定。</para>
/// <para><b>时间戳形态</b>：官方参数表标 <c>invited_time</c> / <c>bind_time</c> 为 number（unix 秒），
/// 但回包示例值写成带引号字符串——表与示例冲突时本仓以参数表为准建 <c>long?</c> 并留档（宿主如遇字符串形态须自行容错）。</para>
/// <para><b>敏感面</b>：<c>openid</c> 与 <c>nickname</c> 属个人敏感信息，禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerListResponse : WxaResponse
{
    /// <summary>已绑定分享员列表（<c>sharers</c>，object array）。</summary>
    [JsonPropertyName("sharers")]
    public List<WxaShopSharerInfo>? Sharers { get; set; }

    /// <summary>绑定的分享员总数（<c>total_num</c>，number）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }
}
