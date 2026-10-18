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
/// 批量邀请分享员请求（<c>POST /shop/sharer/bind</c>，请求体只有 <c>openids</c> 一支键、平铺无包装）。
/// </summary>
/// <remarks>
/// <para><b>这是「邀请」不是「绑定」</b>：调用成功仅代表邀请模版消息下发成功（处于邀请中状态），
/// 用户同意后才完成绑定，最终结果由官方回调 <c>sharer_bind_staus_change</c>（官方页面拼写即 <c>staus</c>）推送。</para>
/// <para><b>邀请频率上限（业务上限、非端点频率）</b>：同一分享员一天最多邀请三次、10 分钟内最多邀请一次，
/// 超限回包 <c>result_list[].reason_code=204</c>；小程序绑定分享员总数上限 10000（<c>reason_code=200</c>）。</para>
/// <para><b>敏感面</b>：<c>openid</c> 为用户标识（个人敏感信息），禁止写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerBindRequest
{
    /// <summary>要邀请的分享员 openid 列表（<c>openids</c>，string array，必填）。禁落日志。</summary>
    [JsonPropertyName("openids")]
    public List<string>? OpenIds { get; set; }
}
