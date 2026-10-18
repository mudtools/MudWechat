// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.BusinessCapability;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 行业能力·B2b 门店助手（3 端点：批量预录入门店 + 门店信息查询 + 门店模板消息下发；
/// 路由族 <c>/wxa/business/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档树两处</b>：门店录入与查询位于
/// <c>miniprogram/dev/server/API/B2b/store_assistant/</c>；模板消息下发位于
/// <c>miniprogram/dev/server/API/B2b/notify/</c>。旧
/// <c>platform-capabilities/industry/B2b_store_assistant.html</c> 为接入指引，不作为出处。
/// </para>
/// <para><b>权限集 158</b>；全域复用小程序 <c>access_token</c>（Query 注入）。</para>
/// <para>
/// <b>敏感面</b>：手机号、统一社会信用代码、法人 / 负责人姓名、<c>openid</c>
/// 一律禁入日志、遥测与异常消息。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "BusinessCapability", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaBusinessRetailService
{
    /// <summary>
    /// 批量预录入门店信息（<c>POST /wxa/business/batchcreateretail</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/B2b/store_assistant/api_batchcreateretail.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaBusinessBatchCreateRetailRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败条数与失败明细，见 <see cref="WxaBusinessBatchCreateRetailResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>部分成功</b>：单次 ≤100 条，<c>errcode=0</c> 仍可能含失败项 ⇒ 必须结合
    /// <c>num_failure</c> 与 <c>failure_record_list</c> 复核，不可仅以 <c>errcode</c> 判定全量成功。
    /// </para>
    /// <para>失败重试须按 <c>mobile_phone</c> / <c>registration_number</c> 去重，避免重复预录入。</para>
    /// </remarks>
    [Post("/wxa/business/batchcreateretail")]
    Task<WxaBusinessBatchCreateRetailResponse> BatchCreateRetailAsync(
        [Body] WxaBusinessBatchCreateRetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询门店信息（<c>POST /wxa/business/getretailinfo</c>，查询类端点官方仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/B2b/store_assistant/api_getretailinfo.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaBusinessGetRetailInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>门店信息数组，见 <see cref="WxaBusinessGetRetailInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b><c>openid</c> 与 <c>mobile_phone</c> 二选一</b>，两者同时给时以 <c>openid</c> 为准。</para>
    /// <para>
    /// <b>官方字段表与示例结构不一致（诚实留档）</b>：字段表平铺门店字段，示例却包裹在
    /// <c>info</c> 数组内 ⇒ 本 SDK 按示例建模为 <c>info</c> 数组。
    /// </para>
    /// </remarks>
    [Post("/wxa/business/getretailinfo")]
    Task<WxaBusinessGetRetailInfoResponse> GetRetailInfoAsync(
        [Body] WxaBusinessGetRetailInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 向门店下发模板消息（<c>POST /wxa/business/retailnotifybusiness</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/B2b/notify/api_retailnotifybusiness.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaBusinessRetailNotifyBusinessRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>统一错误信封，见 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para><b>接收人上限 200</b>；<c>content</c> 为与 <c>type</c> 模板匹配的 JSON 字符串。</para>
    /// <para><b>非幂等</b>：重复下发会重复触达用户。</para>
    /// </remarks>
    [Post("/wxa/business/retailnotifybusiness")]
    Task<WxaResponse> RetailNotifyBusinessAsync(
        [Body] WxaBusinessRetailNotifyBusinessRequest request,
        CancellationToken cancellationToken = default);
}
