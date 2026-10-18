// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」域——店铺入驻与资质面接口（4 端点：注册 / 提交主体资料 / 提交基础资料 / 查询审核状态）。
/// </summary>
/// <remarks>
/// <para><b>令牌形态（MP-X2 / MP-X3 同口径）</b>：本域以小程序 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c> 声明，<b>不新增令牌类型</b>。</para>
/// <para>
/// <b>令牌名分叉（本域最大的反直觉点，守卫 PC4 锁定）</b>：<c>register_shop</c> 与 <see cref="CheckAuditStatusAsync"/> 之外的两支
/// （<c>submit_basicinfo</c> / <c>submit_merchantinfo</c> / <c>check_audit_status</c>）官方请求地址以 <c>access_token</c> 为凭据，
/// 而 <see cref="RegisterShopAsync"/> 官方请求地址写的是 <c>component_access_token</c>（服务商代入驻形态）。
/// 本 SDK 仍按 <c>access_token</c> 注入（代调用形态由宿主以小程序应用配置供给令牌，component 令牌面归开放平台线），
/// 该分叉只在各端点 remarks 留档，<b>不</b>引入第二种令牌注入名。
/// </para>
/// <para>
/// <b>敏感个人信息（AGENTS §8）</b>：本域请求体含<b>身份证姓名 / 身份证号码 / 手机号 / 邮箱 / 银行账号 / 法人姓名</b>等个人金融与敏感个人信息，
/// 应答 <c>legal_validation_url</c> / <c>sign_url</c> 是可完成签约的凭据链接 ⇒ <b>严禁</b>写入日志、遥测或异常消息，也禁止参与缓存与埋点上报。
/// </para>
/// <para><b>非幂等</b>：注册与提交类端点是「入驻流程推进」动作，<c>register_shop</c> 实名不匹配即返回 <c>10005</c> ⇒ 不得放进自动重试管道。</para>
/// <para><b>频率</b>：官方四页均未给端点级频率数字；仅 <c>submit_merchantinfo</c> 错误码表含 <c>429</c>「请降低调用频率」与 <c>400</c>「参数错误」。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductCatalog", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductRegisterService
{
    /// <summary>
    /// 提交开户基本资料（<c>POST /product/register/submit_basicinfo</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/register/submit_basicinfo.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>appid</c> / <c>name_info</c> / <c>return_info</c>），见 <see cref="WxaProductBasicInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/register/submit_basicinfo</c>；顶层 <c>appid</c> / <c>name_info</c> / <c>return_info</c> 三支必填。</para>
    /// <para>
    /// <b>命名规则内嵌在字段说明里</b>（照官方原文建模，宿主须自行满足）：小程序名称 4–30 字符（中文计 2）、简称 4–10 字符、
    /// 介绍 4–120 字符且「一个月内可申请 5 次修改」⇒ 这些是<b>规则约束</b>而非 SDK 校验。
    /// </para>
    /// <para><b><c>naming_other_stuff</c> 条件必填</b>：当返回 <c>210047</c> 时才必填，元素为 media_id 数组（须先经图片上传接口换取）。</para>
    /// <para><b>本域地址结构 <c>address_info</c> 与运费模板不同源</b>：此处是「收货地址」（含 <c>user_name</c> / <c>tel_number</c> 等 8 字段），
    /// 而运费模板的 <c>address_info</c> 只有省市县三字段 ⇒ 两套 DTO 不得复用（守卫 PC4 锁定）。</para>
    /// <para><b>错误码 <c>-15</c>「缺少类目资质」</b>是本端点独有码；<c>user_name</c> / <c>tel_number</c> 属个人信息，禁入日志。</para>
    /// </remarks>
    [Post("/product/register/submit_basicinfo")]
    Task<WxaResponse> SubmitBasicInfoAsync(
        [Body] WxaProductBasicInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交开户主体资料（<c>POST /product/register/submit_merchantinfo</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/register/submit_merchantinfo.html"/>。
    /// </summary>
    /// <param name="request">请求体（营业执照 / 组织机构 / 身份证 / 超级管理员 / 结算账户等多层对象），见 <see cref="WxaProductMerchantInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/register/submit_merchantinfo</c>；这是本域字段面最深的一支（八类子对象，全部经官方请求表逐行建模）。</para>
    /// <para>
    /// <b>材料一律先换 media_id</b>：所有证件图片（<c>pic_file</c> / <c>portrait_pic_file</c> / <c>nation_pic_file</c> /
    /// <c>contact_id_doc_copy</c> / <c>business_authorization_letter</c> / <c>special_qualification.pic_file_list</c>）
    /// 都须先经官方「上传图片接口」上传换 <c>media_id</c>，SDK 不代传；<c>pic_file</c> 另含 <c>pay_media_id</c>（支付侧图片 id）。
    /// </para>
    /// <para>
    /// <b>类型口径纠正（守卫 PC4 锁定）</b>：官方把超级管理员的 <c>id_card_number</c> 与 <c>phone</c> 类型列标为 <c>number</c>，
    /// 但两者是身份证号码 / 11 位手机号这类<b>标识符与敏感个人信息</b> ⇒ 建为 <c>string?</c>，不得数值化（否则丢失前导位、且把 PII 当数值处理）。
    /// </para>
    /// <para><b>日期口径混用留档</b>：<c>start_date</c> / <c>end_date</c> 官方示例是 <c>2014-01-01</c> 字符串，「长期」为合法值 ⇒ 建 <c>string?</c>；
    /// 而 <c>settle_acct_info.acct_type</c>（74 对公 / 75 对私）与 <c>applyment_version</c>（2022-06-30 后须填 1）是另一类取值。</para>
    /// <para><b>敏感面</b>：<c>id_card_number</c>、<c>legal_representative</c>、<c>bank_account_no</c>、<c>phone</c>、<c>registration_num</c>
    /// 一律禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/product/register/submit_merchantinfo")]
    Task<WxaResponse> SubmitMerchantInfoAsync(
        [Body] WxaProductMerchantInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询审核状态（<c>POST /product/register/check_audit_status</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/register/check_audit_status.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>wx_name</c> 必填），见 <see cref="WxaProductAuditStatusCheckRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审核状态聚合，见 <see cref="WxaProductAuditStatusCheckResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/register/check_audit_status</c>；请求体只有 <c>wx_name</c>（微信号，必填）。</para>
    /// <para>应答包在 <c>data</c> 下，是<b>五路状态聚合</b>：注册状态 <c>register_status</c>（0 成功 / 1 已发协议未签约 / 2 需重发协议）、
    /// 商家信息 <c>merchant_info_status</c>、账户验证 <c>acct_verify_status</c>、基础信息 <c>basic_info_status</c>、支付签约 <c>pay_sign_status</c>，
    /// 后四路官方仅写「具体含义查看状态枚举值」而<b>本页未内联取值表</b> ⇒ 本仓照实建模为 <c>long?</c> 并在注释标注取值语义未公开，不得臆造枚举。</para>
    /// <para><c>pay_audit_detail</c>（<c>param_name</c> + <c>reject_reason</c>）是支付资质驳回原因；<c>legal_validation_url</c> / <c>sign_url</c> 是可完成法人验证 / 支付签约的凭据链接 ⇒ <b>禁入日志与遥测</b>。</para>
    /// </remarks>
    [Post("/product/register/check_audit_status")]
    Task<WxaProductAuditStatusCheckResponse> CheckAuditStatusAsync(
        [Body] WxaProductAuditStatusCheckRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 注册小商店（<c>POST /product/register/register_shop</c>，官方请求地址凭据名为 <c>component_access_token</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/register/register_shop.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>wx_name</c> / <c>id_card_name</c> / <c>id_card_number</c> / <c>api_openstore_type</c> 必填），见 <see cref="WxaProductRegisterShopRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/register/register_shop</c>；<c>api_openstore_type</c> 官方只给出取值 <c>2</c>（组件开放）。</para>
    /// <para>
    /// <b>令牌名分叉</b>：官方请求地址原文为 <c>?component_access_token=</c>（服务商代入驻形态）。本 SDK 仍以小程序 <c>access_token</c> 声明
    /// （不引入 component 令牌注入名），代调用形态由宿主供给令牌 ⇒ 该分叉与 <see cref="CheckAuditStatusAsync"/> 的 <c>access_token</c> 页并存是本域事实，
    /// 由守卫 PC4 留档，不得据此改写 <c>[Token]</c> 注入名或扩充组件脱敏词表。
    /// </para>
    /// <para><b>实名认证一次匹配</b>：<c>id_card_name</c> / <c>id_card_number</c> / <c>wx_name</c> 三者须实名一致，否则返回 <c>10005</c> 实名认证失败 ⇒ 非幂等，不得重试。</para>
    /// <para><b>敏感面</b>：<c>id_card_name</c> 与 <c>id_card_number</c> 是身份证姓名与号码（敏感个人信息），<b>严禁</b>写入日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/product/register/register_shop")]
    Task<WxaResponse> RegisterShopAsync(
        [Body] WxaProductRegisterShopRequest request,
        CancellationToken cancellationToken = default);
}
