// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「代小程序管理 · 违规记录与申诉」子域 SDK（<b>2 端点</b>：查询违规记录 / 查询申诉记录）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：全域以 <c>authorizer_access_token</c> 承载，Query 参数名恒 <c>access_token</c>（2026-10-17 逐页核验）。</para>
/// <para><b>动词</b>：两支官方均为 POST。此处的 <c>record-management</c>（违规/申诉记录查询，<c>/wxa/get*records</c>）与 <c>record</c>（ICP 备案，<c>/wxa/icp/*</c>，见 30-3）为两套不同路由。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MiniProgramManage", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformMpRecordService
{
    /// <summary>
    /// 查询违规记录（<c>getillegalrecords</c>，起止时间选填）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record-management/getIllegalRecords.html"/>。
    /// </summary>
    [Post("/wxa/getillegalrecords")]
    Task<OpenPlatformMpIllegalRecordsResponse> GetIllegalRecordsAsync(
        [Body] OpenPlatformMpIllegalRecordsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询申诉记录（<c>getappealrecords</c>，按 <c>illegal_record_id</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record-management/getAppealRecords.html"/>。
    /// </summary>
    [Post("/wxa/getappealrecords")]
    Task<OpenPlatformMpAppealRecordsResponse> GetAppealRecordsAsync(
        [Body] OpenPlatformMpAppealRecordsRequest request,
        CancellationToken cancellationToken = default);
}
