// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Wecom;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「企业微信关联」域 SDK（1 端点：获取关联账号企微 id）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域职责（官方契约）</b>：获取企业微信在微信小店侧的 <c>wecom_corp_id</c> / <c>wecom_user_id</c>，
/// 可用于店铺 H5 链接 / 二维码 / 口令等 API 作为查询参数（企微归因链路的换取入口）。
/// </para>
/// <para>
/// <b>适用范围（官方页与其他域不同，勿套用）</b>：仅「微信小店带货助手」账号类型可调用；
/// <b>第三方平台调用：不支持</b>（无权限集 id）；<b>云调用：不支持</b>。
/// </para>
/// <para>
/// MUD005 已知接受风险：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Wecom", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsWecomService
{
    /// <summary>
    /// 获取关联账号企微 id（<c>shop_get_wecom_openid</c>，企微 corp_id/user_id ↔ 小店侧 id 换取）。
    /// </summary>
    /// <param name="request">查询请求（<c>corp_id</c> 必填；<c>user_id</c> 选填且须属于该 corp_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>企微在小店侧的映射 id（<c>data.wecom_corp_id</c> 企业微信 id / <c>data.wecom_user_id</c> 企业微信账号 id）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/wecom/api_shop_get_wecom_openid"/></para>
    /// <para>
    /// 官方业务限制：①查询的企业 <c>corp_id</c> 需要是店铺的关联账号，否则查询失败；
    /// ②查询的成员 <c>user_id</c> 需要在微信小店应用的可见范围内，否则查询失败；③本接口无特殊注意事项。
    /// </para>
    /// <para>官方错误码：<c>914910001</c>（参数非法，参考 errmsg）/ <c>914911001</c>（未绑定企业为关联账号，无法查询）/
    /// <c>914911002</c>（查询不到 user_id）/ <c>914911003</c>（企业微信未授权微信小店）。</para>
    /// <para>官方适用范围：仅「微信小店带货助手」账号类型可调用；第三方平台调用不支持（无权限集 id）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/wecom/get_wecom_id")]
    Task<ChannelsWecomIdResponse> GetWecomIdAsync(
        [Body] ChannelsWecomIdRequest request,
        CancellationToken cancellationToken = default);
}
