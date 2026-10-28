// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>机构模式子商户（境外进件）</b>域 3 支端点（进件 / 修改 / 查询）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 跨境侧，2026-10-18 逐页核验）：
/// 进件子商户 <see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013636642"/>、
/// 修改子商户 <see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013636644"/>、
/// 查询子商户 <see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013636643"/>。
/// 三页「是否需要证书」栏均为<b>「是」</b>。</para>
/// <para><b>域名是本族最容易被「顺手对齐」搞错的一处</b>：三页<b>只印</b>
/// <c>apihk.mch.weixin.qq.com</c>（全页检索无 <c>api.mch</c>）⇒ 本接口逐支钉在
/// <see cref="WechatPayV2HttpClientNames.GlobalBaseAddress"/>，<b>不</b>用 APIv2 主域名。
/// 这与「要证书」是两条独立事实：<c>/secapi/mch/*Institutionsub</c> 既不在 <c>api2.mch</c>，
/// 也不在 <c>api.mch</c>。</para>
/// <para><b>主体标识是 <c>app_id</c>（下划线），签名算法只有 MD5</b>：三页均无 <c>nonce_str</c>、
/// 无 <c>sign_type</c>，进件页更在签名行直接写明「默认加密方式为 MD5，且仅支持 MD5 加密」⇒
/// 请勿按「APIv2 通用形状」补 <c>appid</c>/<c>nonce_str</c>/<c>sign_type</c>。</para>
/// <para><b>进件与修改两支的字段表不等价</b>（故 DTO 不共用）：修改面多 <c>sub_mch_id</c>、
/// 少 <c>merchant_remark</c> 与 <c>attach</c>；<c>app_id</c>/<c>mch_id</c>/<c>sub_mch_id</c>
/// 三键官方标「不支持修改」但仍须上传以定位主体。</para>
/// <para><b>覆盖语义</b>：进件页对 <c>h5_domains</c> 明示「提交的修改的域名列表会覆盖已有支付域名列表」
/// ⇒ H5 域名须<b>全量</b>提交，不做增量。</para>
/// <para><b>个人信息</b>：法人／负责人证件号、注册证件编号、联系人手机邮箱、结算银行账户
/// 官方<b>未</b>标「敏感字段」（仅靠双向 TLS 保护），SDK 侧一律按个人信息处理：不落日志、
/// 不入异常消息（PAY-B7 同线口径）。</para>
/// <para><b>上游源文缺陷留痕</b>（照抄字段表、不「修正」）：修改页 <c>h5_domains</c> 的描述被误粘成
/// 「子商户的结算银行账户信息」；其 XML 示例重复了 <c>mch_id</c> 两行并发出了字段表里没有的
/// <c>merchant_remark</c>；该页错误码印 <c>SYSTEM_ERROR</c> 而另两页印 <c>SYSTEMERROR</c>；
/// 进件页 <c>verification_status</c> 表格写 <c>Under review</c>、示例写 <c>Under Review</c>。</para>
/// </remarks>
public interface IWechatPayV2InstitutionSubService
{
    /// <summary>
    /// 进件（新增）机构模式子商户（APIv2，需证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013636642"/>。
    /// </summary>
    /// <param name="request">进件请求体（官方 34 行字段表），见 <see cref="V2InstitutionSubAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>子商户号与审核状态，见 <see cref="V2InstitutionSubAddResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/secapi/mch/addInstitutionsub</c>；XML；MD5；
    /// 域名为 <c>apihk.mch</c>。</para>
    /// <para><b>条件必填由官方规则约束</b>：<c>business_type</c> 取 <c>ONLINE</c>/<c>BOTH</c> 时
    /// <c>app_download</c>／<c>business_website</c>／<c>office_account</c>／<c>mini_program</c>
    /// <b>四项至少传一项</b>；取 <c>OFFLINE</c>/<c>BOTH</c> 时 <c>store_address</c> <b>必传</b>；
    /// <c>apply_h5_payment = YES</c> 时 <c>h5_website_url</c> 与 <c>h5_domains</c> 必填且域名≤5 个。
    /// SDK 不做这些校验（官方回 <c>INVALID_REQUEST</c>），但判定责任在商户侧。</para>
    /// <para><b>审核 ≠ 可交易</b>：<c>verification_status = Under review</c> 时子商户<b>暂无交易权限</b>，
    /// 须待审核通过；判定须忽略大小写与空格（官方两种写法并存）。</para>
    /// <para><b>证件照片先上传</b>：<c>registration_certificate_copy</c>／<c>store_photos</c>
    /// 取 <see cref="IWechatPayV2MerchantMediaService.UploadMediaAsync"/> 返回的 <c>media_id</c>。</para>
    /// </remarks>
    Task<V2InstitutionSubAddResponse> AddAsync(
        V2InstitutionSubAddRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改机构模式子商户（APIv2，需证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013636644"/>。
    /// </summary>
    /// <param name="request">修改请求体（官方 33 行字段表，含 <c>sub_mch_id</c>），见 <see cref="V2InstitutionSubModifyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>子商户号与 H5 权限状态，见 <see cref="V2InstitutionSubResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/secapi/mch/modifyInstitutionsub</c>；XML；MD5；<c>apihk.mch</c>。</para>
    /// <para><b>未过审不可改</b>：本页独有错误分支「<c>INVALID_REQUEST</c>｜H5支付权限申请失败｜
    /// 子商户未通过审核，无法修改。」⇒ 修改前须先查询确认状态。</para>
    /// <para><b>改 H5 域名要把 <c>apply_h5_payment</c> 填回 YES</b>：官方原文
    /// 「如已有 H5 支付权限但需要修改 H5 支付域名，需要填写为 YES」，否则域名不会被更新。</para>
    /// <para><b>应答无 <c>verification_status</c></b>：本支只回 <c>sub_mch_id</c> 与
    /// <c>h5_authorization_state</c>，审核进展另走 <see cref="QueryAsync"/>。</para>
    /// </remarks>
    Task<V2InstitutionSubResponse> ModifyAsync(
        V2InstitutionSubModifyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询机构模式子商户（APIv2，需证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013636643"/>。
    /// </summary>
    /// <param name="request">查询请求体（官方就 4 行、全部必填），见 <see cref="V2InstitutionSubQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>子商户全量档案与状态，见 <see cref="V2InstitutionSubQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/secapi/mch/queryInstitutionsub</c>；XML；MD5；<c>apihk.mch</c>。</para>
    /// <para><b>状态判定的三套字段</b>：本支回 <c>sub_mch_status</c>
    /// （<c>OPERATING</c>/<c>PENDING</c>/<c>DEACTIVATED</c>/<c>CLOSED</c>/<c>INCOMPLETE_APPLICATION</c>）
    /// 与 <c>application_status</c>（<c>UNDER_REVIEW</c>/<c>REJECTED</c>），
    /// <b>不回</b>进件页的 <c>verification_status</c>。</para>
    /// <para><b>驳回详情是 JSON 数组文本</b>：<c>application_reject_detail</c> 形如
    /// <c>[{"field":"…","reason":"…"}]</c>，本 SDK 原样交回字符串、不猜结构。</para>
    /// <para><b>返回明文个人信息</b>：证件号／手机／邮箱／结算账户以明文回传 ⇒ 不得写日志、
    /// 不得入异常消息。</para>
    /// </remarks>
    Task<V2InstitutionSubQueryResponse> QueryAsync(
        V2InstitutionSubQueryRequest request, CancellationToken cancellationToken = default);
}
