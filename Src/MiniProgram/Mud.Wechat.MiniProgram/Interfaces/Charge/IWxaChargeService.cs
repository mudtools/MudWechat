// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「付费管理」域 SDK（2 端点：资源包用量查询 + 最近用量查询）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 付费管理，2026-10-10 依据官方清单核验）：
/// <c>charge/api_getusagedetail.html</c>（查询已购买资源包的用量情况）、
/// <c>charge/api_getrecentaverageusage.html</c>（获取某个付费能力的最近三个月平均用量）。
/// </para>
/// <para>
/// <b>能力口径（官方原文）</b>：付费能力是平台上的增值能力（如短信、OCR 等），按资源包购买；
/// 本域两个端点分别查询资源包<b>用量</b>与<b>最近三个月平均用量</b>（供容量规划）。
/// </para>
/// <para>
/// <b>令牌与鉴权</b>：全部走应用级 <c>access_token</c>（Query）。两端点官方均为 <b>POST</b>（请求体可为空对象）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Charge", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaChargeService
{
    /// <summary>
    /// 查询购买资源包的用量情况。官方文档：<c>charge/api_getusagedetail.html</c>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>已购买资源包用量列表，见 <see cref="DataModels.Charge.WxaChargeUsageResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/wxa/charge/usage/get</c>（请求体可为空对象）；Query 携带 <c>access_token</c>。</remarks>
    [Post("/wxa/charge/usage/get")]
    Task<DataModels.Charge.WxaChargeUsageResponse> GetUsageAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取某个付费能力的最近用量数据。官方文档：<c>charge/api_getrecentaverageusage.html</c>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>各付费能力最近三个月平均用量，见 <see cref="DataModels.Charge.WxaChargeRecentAverageResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/wxa/charge/usage/get_recent_average</c>（请求体可为空对象）；Query 携带 <c>access_token</c>。</remarks>
    [Post("/wxa/charge/usage/get_recent_average")]
    Task<DataModels.Charge.WxaChargeRecentAverageResponse> GetRecentAverageUsageAsync(
        CancellationToken cancellationToken = default);
}