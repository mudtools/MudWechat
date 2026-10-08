// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Pay;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「企业支付」模块创建对外收款账户域企业自建应用 SDK
/// （提交创建对外收款账户的申请单 + 查询申请单状态 + 提交图片）。
/// <para>
/// 官方仅向自建应用开放本域 3 个端点（代开发应用与第三方应用均暂不支持），
/// 全部声明于本接口（形态对齐 <see cref="IWechatWorkInternalKfKnowledgeService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 自建应用须配置到「对外收款 - 可调用接口的应用」中。
/// </para>
/// <para>
/// 官方约束：申请单与申请单状态查询均仅针对该应用本身提交的申请单；
/// 同一提现人员最多申请 200 个商户号，同一企业最多申请 2000 个商户号。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Pay",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkPayMchApplyService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalPayMchApplyService : IWechatWorkPayMchApplyService
{
    /// <summary>
    /// 提交创建对外收款账户的申请单
    /// <para>企业通过本接口递交材料以创建对外收款账户申请单；
    /// 仅能操作该应用本身提交的申请单。</para>
    /// <para>官方业务限制：图片类字段均须先经「提交图片」接口获取图片 ID，
    /// 且只能使用该应用本身提交的图片；同一提现人员最多申请 200 个商户号，
    /// 同一企业最多申请 2000 个商户号。</para>
    /// </summary>
    /// <param name="request">进件申请请求体（<see cref="ApplyPayMchRequest"/>：
    /// 主体类型 / 营业执照 / 证件信息 / 超级管理员 / 结算账户 / 经营场景等完整材料）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>提交结果（errcode/errmsg 与参数错误详情 error_description）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98973"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/miniapppay/apply_mch")]
    Task<ApplyPayMchResponse> ApplyMchAsync(
        [Body] ApplyPayMchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询申请单状态
    /// <para>查询已提交的创建对外收款账户申请单状态（申请状态 / 签约状态 / 驳回原因 /
    /// 签约与法人验证链接 / 汇款账户验证信息），
    /// 使用提交申请单时填写的 out_request_no 查询，仅能查询该应用本身提交的申请单。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetPayApplymentStatusRequest"/>：out_request_no，1~32 个字符）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>申请单状态（status：applyment_state / sign_state / sign_url / sub_mchid /
    /// audit_detail / account_validation / legal_validation_url）与申请阶段（apply_state）、
    /// 签约阶段（real_sign_state）、驳回理由（reject_reason）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98974"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/miniapppay/get_applyment_status")]
    Task<GetPayApplymentStatusResponse> GetApplymentStatusAsync(
        [Body] GetPayApplymentStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交图片
    /// <para>上传图片得到图片 ID（open_wx_pay_media_id），用于在提交创建对外收款账户申请单时
    /// 填写图片相关字段；图片按应用隔离，不能使用其他应用提交的图片 ID。</para>
    /// <para>官方业务限制：图片文件大小应在 5B ~ 2MB 之间，仅支持 JPG/JPEG、PNG、BMP 格式；
    /// 图片 ID 距离上传超过 30 天、或已被用于成功提交申请单且所有相关申请单均走完流程时立即失效。</para>
    /// </summary>
    /// <param name="formData">multipart 表单内容（请求体为 multipart/form-data，由调用方构建
    /// <see cref="IFormContent"/> 实现，含 filename / filelength / content-type 信息）。
    /// 须标 <c>[MultipartForm]</c> —— 生成器 3.0.1 对 <c>[FormContent]</c> 发射的
    /// <c>GetFormDataContentAsync</c> 调用在运行时接口上不存在（CS1061），
    /// <c>[MultipartForm]</c> 才走 <see cref="IFormContent.ToHttpContentAsync"/> 通路
    /// （形态对齐 <see cref="IWechatWorkExternalContactAttachmentService.UploadAttachmentAsync"/> 先例）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>上传后得到的图片 ID（open_wx_pay_media_id，30 天后过期）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98972"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/miniapppay/upload_image")]
    Task<UploadPayImageResponse> UploadImageAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);
}
