// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「代小程序管理 · ICP 备案」子域 SDK（<b>13 端点</b>：提交/取消备案、人脸核身任务、备案入口与在途订单查询、证件类型/前置审批类型/主体类型/服务内容类型/行政区划字典查询、备案媒体材料上传）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：全域以 <c>authorizer_access_token</c> 承载，Query 参数名恒 <c>access_token</c>（2026-10-17 逐页取证）。</para>
/// <para><b>动词陷阱</b>：<c>query_icp_verifytask</c> 名义为查询、官方实为 <b>POST</b>（带 JSON 请求体），与本域其余 <c>query_*</c>/<c>get_*</c> 的 GET 不同，勿望文生义。</para>
/// <para><b>形态二分</b>：本域 JSON 面 12 支走 <c>[Body]</c> DTO，<c>upload_icp_media</c> 一支走 <c>[MultipartForm] IFormContent</c>（官方要求 <c>multipart/form-data</c>）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MiniProgramManage", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformMpIcpService
{
    /// <summary>
    /// 提交备案申请（<c>apply_icp_filing</c>，字段校验失败经 <c>hints</c> 逐项返回）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/applyIcpFiling.html"/>。
    /// </summary>
    [Post("/wxa/icp/apply_icp_filing")]
    Task<OpenPlatformMpIcpApplyResponse> ApplyIcpFilingAsync(
        [Body] OpenPlatformMpIcpApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消备案申请（<c>cancel_apply_icp_filing</c>，撤销审核中的申请，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/cancelApplyIcpFiling.html"/>。
    /// </summary>
    [Post("/wxa/icp/cancel_apply_icp_filing")]
    Task<OpenPlatformResponse> CancelApplyIcpFilingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 注销备案（<c>cancel_icp_filing</c>，注销已完成的备案）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/cancelIcpfiling.html"/>。
    /// </summary>
    [Post("/wxa/icp/cancel_icp_filing")]
    Task<OpenPlatformResponse> CancelIcpFilingAsync(
        [Body] OpenPlatformMpIcpCancelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建人脸核身任务（<c>create_icp_verifytask</c>，返回核身链接与任务 id）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/createIcpVerifyTask.html"/>。
    /// </summary>
    [Post("/wxa/icp/create_icp_verifytask")]
    Task<OpenPlatformMpIcpCreateVerifyTaskResponse> CreateIcpVerifyTaskAsync(
        [Body] OpenPlatformMpIcpCreateVerifyTaskRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询人脸核身任务（<c>query_icp_verifytask</c>；<b>注意官方动词为 POST</b>，非 GET）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/queryIcpVerifyTask.html"/>。
    /// </summary>
    [Post("/wxa/icp/query_icp_verifytask")]
    Task<OpenPlatformMpIcpVerifyTaskResponse> QueryIcpVerifyTaskAsync(
        [Body] OpenPlatformMpIcpQueryVerifyTaskRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取备案入口信息（<c>get_icp_entrance_info</c>，备案状态与可执行操作）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/getIcpEntranceInfo.html"/>。
    /// </summary>
    [Get("/wxa/icp/get_icp_entrance_info")]
    Task<OpenPlatformMpIcpEntranceInfoResponse> GetIcpEntranceInfoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取正在备案中的订单（<c>get_online_icp_order</c>，回显在途备案的主体与小程序信息）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/getOnlineIcpOrder.html"/>。
    /// </summary>
    [Get("/wxa/icp/get_online_icp_order")]
    Task<OpenPlatformMpOnlineIcpOrderResponse> GetOnlineIcpOrderAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询证件类型（<c>query_icp_certificate_types</c>，备案字典）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/queryIcpCertificateTypes.html"/>。
    /// </summary>
    [Get("/wxa/icp/query_icp_certificate_types")]
    Task<OpenPlatformMpIcpCertificateTypesResponse> QueryIcpCertificateTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询前置审批类型（<c>query_icp_nrlx_types</c>，备案字典）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/queryIcpNrlxTypes.html"/>。
    /// </summary>
    [Get("/wxa/icp/query_icp_nrlx_types")]
    Task<OpenPlatformMpIcpNrlxTypesResponse> QueryIcpNrlxTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询主体类型（<c>query_icp_subject_types</c>，备案字典）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/queryIcpSubjectTypes.html"/>。
    /// </summary>
    [Get("/wxa/icp/query_icp_subject_types")]
    Task<OpenPlatformMpIcpSubjectTypesResponse> QueryIcpSubjectTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询服务内容类型（<c>query_icp_service_content_types</c>，备案字典）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/queryIcpServiceContentTypes.html"/>。
    /// </summary>
    [Get("/wxa/icp/query_icp_service_content_types")]
    Task<OpenPlatformMpIcpServiceContentTypesResponse> QueryIcpServiceContentTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询行政区划（<c>query_icp_district_code</c>，树形自递归字典）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/queryIcpDistrictCode.html"/>。
    /// </summary>
    [Get("/wxa/icp/query_icp_district_code")]
    Task<OpenPlatformMpIcpDistrictCodeResponse> QueryIcpDistrictCodeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传备案媒体材料（<c>upload_icp_media</c>，<c>multipart/form-data</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/uploadIcpMedia.html"/>。
    /// </summary>
    /// <param name="formData">
    /// multipart 表单内容（由调用方构建 <see cref="IFormContent"/> 实现并标 <c>[MultipartForm]</c>；
    /// 官方表单字段四支、名称须照原文：<c>type</c> 必填（<c>image</c> / <c>video</c>）、<c>certificate_type</c> 可选（上传证件材料时必填，须与 <c>icp_order_field</c> 对应，否则报 86201 或使用时报错）、
    /// <c>icp_order_field</c> 必填（媒体材料所属备案字段名，多字段可复用时填其中一个）、<c>media</c> 必填（图片或视频的二进制内容）。
    /// 图片仅 JPG/JPEG/PNG 且 ≤2MB，视频仅 MP4）。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformMpUploadIcpMediaResponse"/>（<c>media_id</c> 填入申请备案接口的对应字段）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/icp/upload_icp_media</c> + <c>multipart/form-data</c>；官方 <c>Content-Type</c> 为 multipart，JSON DTO 通路无法承载二进制字段 ⇒ 表单由宿主拼装，守卫 MPG11 断言本端点无 JSON 请求 DTO。</para>
    /// <para>Query 携带 <c>access_token</c>（值为 <c>authorizer_access_token</c>，权限集 18、156）。</para>
    /// <para>官方错误码：<c>-1</c> 系统繁忙 / <c>86200</c> 内部错误 / <c>86201</c> 上传失败。</para>
    /// </remarks>
    [Post("/wxa/icp/upload_icp_media")]
    Task<OpenPlatformMpUploadIcpMediaResponse> UploadIcpMediaAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);
}
