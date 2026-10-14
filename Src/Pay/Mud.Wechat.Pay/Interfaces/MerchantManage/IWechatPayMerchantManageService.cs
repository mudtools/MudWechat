// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.MerchantManage;

namespace Mud.Wechat.Pay.Interfaces.MerchantManage;

/// <summary>
/// 商户管理记录服务接口（3 声明式端点 + 1 multipart 手工通道）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方开放面</b>：四页逐字段核验（更新于 2025.07.18），支持商户均为【普通服务商】。
/// </para>
/// <para>
/// <b>关键形态</b>：① 提交资料支为 POST + 204 No Content；② <c>submit_data</c> 官方类型
/// <c>string(4000000)</c>（JSON 序列化后的字符串，非对象）；③ 分页查询支的 <c>limit</c> 为必填参数（上限 10）；
/// ④ multipart 上传通道的表单字段名为 <c>submission_file</c> + <c>meta</c>（与媒体上传的 <c>file</c> + <c>meta</c> 不同）。
/// </para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MerchantManage", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMerchantManageService
{
    /// <summary>
    /// 查询商户管理记录详情（<c>GET /v3/mch-manage/mch-manage-records/{manage_record_id}</c>）。
    /// </summary>
    /// <param name="manageRecordId">商户管理记录 ID（path 参数）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>管理记录详情。</returns>
    /// <remarks>
    /// <para><b>官方页面</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940383">4014940383</see></para>
    /// <para><b>成功应答</b>：200 + JSON（含 <c>submit_data</c> 字符串）。</para>
    /// </remarks>
    [Get("/v3/mch-manage/mch-manage-records/{manageRecordId}")]
    Task<MerchantManageRecordQueryResponse> GetManageRecordAsync(
        [Path] long manageRecordId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交商户管理记录资料（<c>POST /v3/mch-manage/mch-manage-records/{manage_record_id}/mch-manage-submissions</c>）。
    /// </summary>
    /// <param name="manageRecordId">商户管理记录 ID（path 参数）。</param>
    /// <param name="request">提交请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>204 No Content（无包体）。</returns>
    /// <remarks>
    /// <para><b>官方页面</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940417">4014940417</see></para>
    /// <para><b>成功应答</b>：204（本域唯一的写操作，无返回内容）。</para>
    /// </remarks>
    [Post("/v3/mch-manage/mch-manage-records/{manageRecordId}/mch-manage-submissions")]
    Task SubmitManageRecordSubmissionAsync(
        [Path] long manageRecordId,
        MerchantManageSubmissionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按子商户号查询管理记录列表（<c>GET /v3/mch-manage/mch-manage-records/sub-mchid/{sub_mchid}</c>）。
    /// </summary>
    /// <param name="subMchId">子商户号（path 参数）。</param>
    /// <param name="limit">每页数量（query 参数，必填，上限 10）。</param>
    /// <param name="offset">偏移量（query 参数，选填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>管理记录列表及总数。</returns>
    /// <remarks>
    /// <para><b>官方页面</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940381">4014940381</see></para>
    /// <para><b>成功应答</b>：200 + JSON（数组形态）。</para>
    /// </remarks>
    [Get("/v3/mch-manage/mch-manage-records/sub-mchid/{subMchId}")]
    Task<MerchantManageRecordsBySubMchIdResponse> GetManageRecordsBySubMchIdAsync(
        [Path] string subMchId,
        [Query] int limit,
        [Query] int? offset = null,
        CancellationToken cancellationToken = default);
}
