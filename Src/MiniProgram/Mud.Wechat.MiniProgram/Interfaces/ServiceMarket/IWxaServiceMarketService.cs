// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「微信服务市场」域 SDK（2 端点：调用服务市场接口 + 异步获取处理数据）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 微信服务市场，2026-10-10 依据官方清单核验）：
/// <c>wx-service-market/api_invokeservice.html</c>、<c>wx-service-market/api_servicemarketretrieve.html</c>。
/// </para>
/// <para>
/// <b>业务形态（官方原文）</b>：服务市场是面向开发者的增值服务（能力）交易/调用平台；
/// 本接口用于调用服务平台上架的 API（适用于公众号、小程序与第三方平台，区别仅在 access_token 的生成）。
/// 调用 <c>async=true</c> 的异步 API 时，先经 <see cref="InvokeAsync"/> 取得 <c>request_id</c>，
/// 再经 <see cref="RetrieveAsync"/> 拉取处理后的数据。
/// </para>
/// <para>
/// <b>令牌与鉴权</b>：全部走应用级 <c>access_token</c>（Query）。两端点官方均为 <b>POST</b>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ServiceMarket", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaServiceMarketService
{
    /// <summary>
    /// 调用服务市场接口。官方文档：<c>wx-service-market/api_invokeservice.html</c>。
    /// </summary>
    /// <param name="request">调用请求（<c>service</c> / <c>api</c> 必填），见 <see cref="DataModels.ServiceMarket.WxaServiceMarketRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>服务方回包（<c>data</c> JSON 字符串；异步场景含 <c>request_id</c>），见 <see cref="DataModels.ServiceMarket.WxaServiceMarketResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/servicemarket</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>参数与失败（官方原文）</b>：<c>service</c>（服务 ID）与 <c>api</c>（接口名）必填；
    /// <c>data</c> 为服务方接口定义的数据对象；服务方业务失败以 <c>errcode</c> 非零表达
    /// （部分服务方错误码与微信 <c>errcode</c> 同域）。<c>data</c> 可能承载业务敏感数据，禁止落日志。
    /// </para>
    /// </remarks>
    [Post("/wxa/servicemarket")]
    Task<DataModels.ServiceMarket.WxaServiceMarketResponse> InvokeAsync(
        [Body] DataModels.ServiceMarket.WxaServiceMarketRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取服务市场处理数据。官方文档：<c>wx-service-market/api_servicemarketretrieve.html</c>。
    /// </summary>
    /// <param name="request">拉取请求（<c>request_id</c> 必填），见 <see cref="DataModels.ServiceMarket.WxaServiceMarketRetrieveRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>异步处理后数据（<c>data</c> JSON 字符串），见 <see cref="DataModels.ServiceMarket.WxaServiceMarketRetrieveResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/servicemarketretrieve</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>时机（官方原文）</b>：调用服务市场接口（<see cref="InvokeAsync"/> 且 <c>async=true</c>）后
    /// 根据返回的 <c>request_id</c> 拉取异步处理结果；<c>data</c> 为服务方回包 JSON 字符串，可能承载业务敏感数据，禁止落日志。</para>
    /// </remarks>
    [Post("/wxa/servicemarketretrieve")]
    Task<DataModels.ServiceMarket.WxaServiceMarketRetrieveResponse> RetrieveAsync(
        [Body] DataModels.ServiceMarket.WxaServiceMarketRetrieveRequest request,
        CancellationToken cancellationToken = default);
}