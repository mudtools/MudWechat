// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopOrder;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 V2 · 微信支付进件」域 SDK（3 端点：申请开通 / 查询申请状态 / 查询进件信息；
/// 官方 <c>minishopopencomponent2/API/wxpay</c> 分区，路由一律 <c>/shop/wxpay/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>；
/// 进件面只跑小程序 <c>access_token</c>，不涉及微信支付 APIv3 的商户私钥/证书链。
/// </para>
/// <para>
/// <b>进件 ≠ 开通</b>：<c>apply</c> 只提交申请，同步回包仅 <c>errcode</c> / <c>errmsg</c>；
/// 真实结果经 <c>get</c> 的 <c>wxpay_resp.applyment_state</c> 轮询（官方注明「参考查询申请状态 API」）与
/// 支付回调/签约链接推进 ⇒ 不得以 <c>errcode=0</c> 当作「微信支付已开通」。
/// </para>
/// <para>
/// <b><c>get</c> 的两页官方口径不一致</b>（同一路由 <c>/shop/wxpay/get</c>）：
/// <c>wxafunds/API/wxpay/get.html</c> 给出请求参数表（<c>biz_type</c> 固定 1、<c>applyment_id</c> /
/// <c>out_request_no</c> 二选一），而 <c>minishopopencomponent2/API/wxpay/get.html</c> 的请求示例是空对象 <c>{}</c>
/// 且只列错误码 <c>1040039</c>（尚未进行支付进件）⇒ 请求 DTO 按<b>全集可空</b>建模，
/// 宿主应优先传 <c>applyment_id</c> 或 <c>out_request_no</c> 并在空回包时按状态查询语义处理。
/// </para>
/// <para>
/// <b>写侧与读侧字段名不同</b>：申请用 <c>wxa_business_license_copy</c> / <c>wxa_id_card_copy</c> /
/// <c>wxa_id_card_national</c>，回显却是 <c>business_license_copy</c> / <c>id_card_copy</c> / <c>id_card_national</c>
/// ⇒ 读写各建 CLR 类型（本仓建模口径，守卫 SHPO-W 锁定），不得复用一个类型。
/// </para>
/// <para>
/// <b>主体与账户约束（官方硬校验）</b>：<c>organization_type</c> 仅 <c>4</c> 个体工商户 / <c>2</c> 企业；
/// <c>account_info.bank_account_type</c> 企业只能 <c>74</c> 对公，个体工商户可 <c>74</c> / <c>75</c> 对私；
/// 开户名称须与身份证姓名（对私）或营业执照商户名称（对公）一致；
/// <c>id_card_valid_time</c> 有效期须大于 60 天；特殊资质与补充材料各最多 5 张。
/// </para>
/// <para>
/// <b>官方端点级频率数字空缺</b>：3 页正文均无「次/分钟」类数字，也未给权限集编号与云调用说明 ⇒ 注释留档，
/// 不得凭第三方 SDK 或记忆补数。
/// </para>
/// <para>
/// <b>敏感面（AGENTS §8）</b>：身份证姓名 / 号码 / 住址、法定代表人、手机号、邮箱、银行账号、
/// 结算账户户名、营业执照号、<c>sub_mchid</c>、<c>sign_url</c> / <c>legal_validation_url</c> 与全部证照
/// <c>media_id</c> 属个人敏感信息或金融凭据 ⇒ <b>一律不得写入日志、遥测或异常消息</b>；
/// 官方回显已对姓名/证件号/手机做过脱敏，脱敏后仍属个人信息。
/// </para>
/// <para>
/// <b>非幂等</b>：<c>apply</c> 每次成功提交都会生成新的进件申请单（以 <c>out_request_no</c> 为商家侧单号），
/// 重复提交会产生新的申请 ⇒ <b>不得</b>放进自动重试管道。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopOrder", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopWxPayService
{
    /// <summary>
    /// 申请开通微信支付（<c>POST /shop/wxpay/apply</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/wxpay/apply.html"/>。
    /// </summary>
    /// <param name="request">进件请求体，见 <see cref="WxaShopWxPayApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（真实进件状态须另行查询）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>organization_type</c> / <c>business_license_info</c> /
    /// <c>id_doc_type</c> / <c>contact_info</c> / <c>account_info</c> / <c>merchant_shortname</c> 必填；
    /// <c>id_card_info</c> 在 <c>IDENTIFICATION_TYPE_MAINLAND_IDCARD</c> 时必填；
    /// <c>id_doc_info</c> 在护照 / 来往通行证时填写；本域官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b><c>organization_type</c> 类型口径</b>：官方参数表标 <c>string</c>，示例却以数字 <c>2</c> 书写 ⇒
    /// 本仓按参数表建模为 <c>string</c>，宿主传 <c>"2"</c> / <c>"4"</c>。
    /// </para>
    /// <para>
    /// <b>校验前置</b>：图片须先经「上传图片」接口换取 <c>media_id</c> 与 <c>pay_media_id</c>；
    /// 证照须彩色扫描 / 彩色数码拍摄、完整清晰、不得二次剪裁或 PS；营业执照号 / 身份证号 / 银行账号等
    /// 属敏感信息，构造请求与异常消息时不得回显。
    /// </para>
    /// <para><b>非幂等</b>：重复提交生成新申请单 ⇒ 不得自动重试。</para>
    /// </remarks>
    [Post("/shop/wxpay/apply")]
    Task<WxaResponse> ApplyAsync(
        [Body] WxaShopWxPayApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询微信支付申请状态（<c>POST /shop/wxpay/get</c>）。
    /// 官方文档（请求参数表出处）：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/wxafunds/API/wxpay/get.html"/>；
    /// 另一官方页（空请求体口径）：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/wxpay/get.html"/>。
    /// </summary>
    /// <param name="request">申请单定位键，见 <see cref="WxaShopWxPayGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>wxpay_req</c>（进件回显）+ <c>wxpay_resp</c>（申请状态），见 <see cref="WxaShopWxPayGetResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>wxafunds</c> 页要求 <c>biz_type</c> 固定 <c>1</c>，
    /// <c>applyment_id</c> 与 <c>out_request_no</c> 二选一；<c>minishopopencomponent2</c> 页示例为 <c>{}</c>；
    /// 本域官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b>状态树</b>：<c>wxpay_resp</c> 含 <c>applyment_state</c> / <c>applyment_state_desc</c> /
    /// <c>sign_url</c>（签约链接）/ <c>sub_mchid</c> / <c>account_validation</c>（打款验证）/
    /// <c>audit_detail[]</c>（驳回明细）/ <c>legal_validation_url</c>（法人验证链接）/ <c>applyment_id</c>。
    /// 错误 <c>1040039</c> 表示尚未进行支付进件。
    /// </para>
    /// <para><b>只读</b>：可安全重试；宿主应按官方建议做退避轮询，避免高频打满。</para>
    /// </remarks>
    [Post("/shop/wxpay/get")]
    Task<WxaShopWxPayGetResponse> GetAsync(
        [Body] WxaShopWxPayGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询微信支付进件信息（<c>POST /shop/wxpay/get_info</c>，官方请求体为空对象 <c>{}</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/wxpay/get_info.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>pay_req</c> 进件信息回显，见 <see cref="WxaShopWxPayGetInfoResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 空对象请求体；<b>本页没有请求 / 返回参数说明表</b>
    /// （只有请求示例、回包示例）⇒ 返回结构取自回包示例、已在此留档，不得按其它页补字段；
    /// 本域官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b>回包键名是 <c>pay_req</c></b>（不是 <c>wxpay_req</c>）；其中姓名 / 证件号 / 手机号按官方示例已脱敏，
    /// 但仍属个人信息，不得进日志。
    /// </para>
    /// <para>官方请求体无任何业务字段 ⇒ 本方法不带 <c>[Body]</c> 参数（守卫 SC3 锁定）。</para>
    /// <para><b>只读</b>：可安全重试。</para>
    /// </remarks>
    [Post("/shop/wxpay/get_info")]
    Task<WxaShopWxPayGetInfoResponse> GetInfoAsync(
        CancellationToken cancellationToken = default);
}
