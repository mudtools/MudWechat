// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 设置测试白名单（<c>POST /card/testwhitelist/set</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>用途</b>：卡券功能在未通过官方审核前仅对白名单用户可见，本端点即该灰度通道。
/// </para>
/// <para>
/// <b>两键二选一</b>：<c>openid</c>（用户）与 <c>username</c>（公众号原始 id / 微信号方向）由官方按场景取用；
/// SDK <b>不做本地互斥校验</b>。对齐基准 SKIT 两字段均为可空 <c>string</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardTestWhiteListSetRequest
{
    /// <summary>获取或设置测试用户 openid（官方 <c>openid</c>，选填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置测试账号名（官方 <c>username</c>，选填；语义<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }
}
