// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Apply4Subject;

/// <summary>
/// 提交主体进件申请单（<c>POST /v3/apply4subject/applyment/</c>，商户开户意愿确认）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012722388"/>
/// （2026-10-14 逐字段核验；支持商户：<b>渠道商</b> + <b>从业机构（银行）</b> + <b>从业机构（支付机构）</b>）。
/// </para>
/// <para>
/// <b>⚠️ 本域与 <c>apply4sub</c>（特约商户进件）是两套互不兼容的契约，不得互相套用</b>：
/// 主体进件是「服务商为<b>公司主体</b>开户 + 开户意愿确认」，报文层级、字段拼写、状态取值全部不同 ——
/// 营业执照字段在本域是 <c>business_licence_info</c>（英式拼写），特约商户进件是
/// <c>business_license_info</c>（美式）；本域的 <c>identification_info</c> 与 <c>ubo_info_list</c>
/// 是<b>顶层</b>对象，后者嵌在 <c>subject_info</c> 内。守卫 A4SJ-B3 逐字段锁死本域拼写。
/// </para>
/// <para>
/// <b>⚠️ 路由结尾的斜杠是官方原文</b>：<c>/v3/apply4subject/applyment/</c> —— 而同资源的
/// <b>查询</b>端点是 <c>/v3/apply4subject/applyment</c>（<b>无</b>尾斜杠）⇒
/// 同一资源路径下 POST 带斜杠、GET 不带，是本域最易「顺手统一」的一处，守卫 A4SJ-B2 锁死。
/// </para>
/// <para>
/// <b>异步受理</b>：返回 <c>applyment_id</c> 只代表申请单已创建（初始态多为
/// <c>APPLYMENT_STATE_EDITTING</c> 或 <c>APPLYMENT_STATE_WAITTING_FOR_AUDIT</c>），最终结果须经
/// <see cref="Apply4SubjectApplymentStateResponse"/> 轮询取得。
/// </para>
/// <para>
/// <b>敏感字段加密</b>：<c>contact_info</c> 的姓名 / 手机号 / 证件号，<c>identification_info</c> 的
/// 姓名 / 证件号 / 证件居住地址，<c>ubo_info_list[]</c> 的姓名 / 证件号 / 居住地址均须用
/// <b>微信支付公钥</b>（推荐）或平台证书公钥以 RSAES-OAEP 加密，并把请求头 <c>Wechatpay-Serial</c>
/// 指向对应公钥 ID / 证书序列号（该头由传输层按配置写入）。
/// </para>
/// <para>
/// <b>示例报文里有 <c>ubo_info</c>（单数）但字段表内没有</b>：官方本页「请求参数」表只列
/// <c>ubo_info_list</c>（复数，且已标「即将下线处理，无需传入」），示例 JSON 里那份
/// <c>ubo_info</c>（<c>ubo_id_type</c> / <c>ubo_id_card_copy</c> / <c>ubo_name</c> …）是旧版遗留
/// ⇒ <b>字段表为权威</b>，本类型<b>刻意不加</b> <c>ubo_info</c>，只建 <see cref="Apply4SubjectUboInfo"/> 供复数列表使用。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4Subject")]
public class Apply4SubjectApplymentRequest
{
    /// <summary>渠道商户号（<c>channel_id</c>，选填 string(32)）：从业机构调用时<b>必填</b>（填其下的渠道商户号），渠道商户自身调用时无需填写。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>业务申请编号（<c>business_code</c>，必填 string(128)）：只能由数字、字母或下划线组成，服务商自定义唯一编号，每个编号对应一个申请单；官方建议前缀带上服务商商户号。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>联系人信息（<c>contact_info</c>，必填对象），见 <see cref="Apply4SubjectContactInfo"/>。</summary>
    [JsonPropertyName("contact_info")]
    public Apply4SubjectContactInfo? ContactInfo { get; set; }

    /// <summary>主体信息（<c>subject_info</c>，必填对象），见 <see cref="Apply4SubjectSubjectInfo"/>。</summary>
    [JsonPropertyName("subject_info")]
    public Apply4SubjectSubjectInfo? SubjectInfo { get; set; }

    /// <summary>
    /// 法定代表人身份信息（<c>identification_info</c>，必填对象），见 <see cref="Apply4SubjectIdentificationInfo"/>。
    /// </summary>
    /// <remarks><b>本域为顶层字段</b>；特约商户进件的同义信息嵌在 <c>subject_info.identity_info</c> 内。</remarks>
    [JsonPropertyName("identification_info")]
    public Apply4SubjectIdentificationInfo? IdentificationInfo { get; set; }

    /// <summary>补充材料（<c>addition_info</c>，选填对象），见 <see cref="Apply4SubjectAdditionInfo"/>。</summary>
    [JsonPropertyName("addition_info")]
    public Apply4SubjectAdditionInfo? AdditionInfo { get; set; }

    /// <summary>
    /// 最终受益人信息列表（<c>ubo_info_list</c>，选填数组），见 <see cref="Apply4SubjectUboInfo"/>。
    /// </summary>
    /// <remarks>官方标注<b>「该字段即将下线处理，无需传入」</b>；照原文保留建模（顶层，最多 4 人），新接入建议不填。</remarks>
    [JsonPropertyName("ubo_info_list")]
    public List<Apply4SubjectUboInfo>? UboInfoList { get; set; }
}

/// <summary>提交主体进件申请单的应答（<b>仅</b> <c>applyment_id</c>）。</summary>
/// <remarks>
/// 官方本页应答字段表只有 <c>applyment_id</c>（integer）⇒ 与特约商户进件的提交应答同为单字段形态，
/// 但两域类型<b>不共用</b>（守卫按命名空间各自断言）；回读业务编号请查
/// <see cref="Apply4SubjectApplymentStateResponse"/> 或按 <c>business_code</c> 查询。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4Subject")]
public class Apply4SubjectApplymentResponse : WechatPayResponse
{
    /// <summary>申请单编号（<c>applyment_id</c>，Integer）：微信支付分配，查询 / 撤销申请单的键。</summary>
    [JsonPropertyName("applyment_id")]
    public long? ApplymentId { get; set; }
}

/// <summary>
/// 主体进件申请单审核结果（<c>GET /v3/apply4subject/applyment</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：查询申请单审核结果 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012697715"/>
/// （2026-10-14 逐字段核验）。
/// </para>
/// <para>
/// <b>与特约商户进件的状态应答形态完全不同</b>：本域只有 4 个字段，
/// <b>没有</b> <c>sub_mchid</c> / <c>sign_url</c> / <c>applyment_state_msg</c> / <c>audit_detail</c>，
/// 也<b>不回显</b> <c>business_code</c> 与 <c>applyment_id</c> ⇒ 两域类型不得合并（守卫 A4SJ-B6）。
/// </para>
/// <para>
/// <b><c>qrcode_data</c> 不是链接而是图片数据</b>：官方标注 base64，解码后为<b>小程序码图片二进制</b>
/// （最长 30000 字符），商家扫码后完成联系信息确认 / 账户验证；出现时机为
/// <c>WAITTING_FOR_CONFIRM_CONTACT</c> / <c>WAITTING_FOR_CONFIRM_LEGALPERSON</c> / <c>PASSED</c> / <c>FREEZED</c> 四态。
/// 这与特约商户进件的 <c>sign_url</c>（给人打开的链接）是两种不同处置，勿写成 URL。
/// </para>
/// <para>
/// <b>驳回定位是单字段</b>：<c>reject_param</c>（被驳回的字段名）+ <c>reject_reason</c>，
/// 仅 <c>REJECTED</c> / <c>FREEZED</c> 返回；比特约商户进件的 <c>audit_detail[]</c> 数组信息量小，
/// 一次只能指出一个驳回项 ⇒ 重试须按轮次逐项修。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4Subject")]
public class Apply4SubjectApplymentStateResponse : WechatPayResponse
{
    /// <summary>申请单状态（<c>applyment_state</c>，必填 string(64)）：取值见 <see cref="Apply4SubjectApplymentStates"/>。</summary>
    [JsonPropertyName("applyment_state")]
    public string? ApplymentState { get; set; }

    /// <summary>小程序码图片（<c>qrcode_data</c>，选填 string(30000)）：<b>base64</b>，解码得图片二进制，供商家扫码完成开户意愿确认。</summary>
    [JsonPropertyName("qrcode_data")]
    public string? QrcodeData { get; set; }

    /// <summary>驳回参数（<c>reject_param</c>，选填 string(256)）：被驳回的字段名，仅 <c>REJECTED</c> / <c>FREEZED</c> 返回。</summary>
    [JsonPropertyName("reject_param")]
    public string? RejectParam { get; set; }

    /// <summary>驳回原因（<c>reject_reason</c>，选填 string(1024)）：仅 <c>REJECTED</c> / <c>FREEZED</c> 返回。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }
}

/// <summary>
/// 商户开户意愿确认状态（<c>GET /v3/apply4subject/applyment/merchants/{sub_mchid}/state</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：获取商户开户意愿确认状态 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012467549"/>
/// （2026-10-14 逐字段核验）。
/// </para>
/// <para>
/// <b>本端点与申请单状态是两个独立维度</b>：这里回答的是「<b>特约商户号</b>有没有完成实名认证」
/// （<see cref="Apply4SubjectAuthorizeStates"/>，2 值），不是申请单审核进度 ⇒
/// <b>不得</b>用 <c>authorize_state</c> 判进件是否通过，也不得用 <c>applyment_state</c> 判授权。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4Subject")]
public class Apply4SubjectMerchantStateResponse : WechatPayResponse
{
    /// <summary>授权状态（<c>authorize_state</c>，string）：取值见 <see cref="Apply4SubjectAuthorizeStates"/>。</summary>
    [JsonPropertyName("authorize_state")]
    public string? AuthorizeState { get; set; }
}

/// <summary>
/// 补充材料（提交申请单 <c>addition_info</c>）。
/// </summary>
/// <remarks>
/// 官方本页 <c>addition_info</c> 内<b>只有</b> <c>confirm_mchid_list</c> 一个字段（选填）——
/// 与特约商户进件的 <c>addition_info</c>（初始经营材料 / 法人持股 / 联系人是否为法人…）字段集<b>完全不同</b>
/// ⇒ 两域类型不共用；照官方原文建模，不加「大概率存在」的字段。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4Subject")]
public class Apply4SubjectAdditionInfo
{
    /// <summary>
    /// 待确认商户号列表（<c>confirm_mchid_list</c>，选填 array&lt;string&gt;，最多 20 个）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 填写后<b>创建申请单即可扫码提前完成授权商户号</b>；官方校验三条硬约束（不满足则<b>创建失败</b>）：
    /// 商户号全称须与申请单主体名称一致、特殊行业商户的结算规则 ID 须与经营许可证匹配、一个申请单最多 20 个。
    /// </para>
    /// <para>
    /// <b>报错格式陷阱</b>：失败时官方在一条消息里以「<c>商户号:原因;商户号:原因</c>」拼接多个商户号
    /// ⇒ 若要剔除报错商户号，官方明确建议<b>按格式（标点）切分商户号，不要按文案筛选</b>（文案后续可能调整）。
    /// </para>
    /// </remarks>
    [JsonPropertyName("confirm_mchid_list")]
    public List<string>? ConfirmMchidList { get; set; }
}
