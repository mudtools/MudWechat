// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Apply4SubMerchant;

/// <summary>
/// 提交特约商户进件申请单（<c>POST /v3/applyment4sub/applyment/</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012719997"/>
/// （2026-10-14 逐字段核验；支持商户：<b>普通服务商</b>）。
/// </para>
/// <para>
/// <b>⚠️ 路由结尾的斜杠是官方原文</b>：<c>/v3/applyment4sub/applyment/</c> —— 删除尾斜杠会得到另一个
/// 资源路径，属真实契约变更而非风格问题，守卫 A4S-B1 锁死。
/// </para>
/// <para>
/// <b>异步受理</b>：本接口返回 <c>applyment_id</c> 只代表<b>申请单已受理</b>，最终结果须经
/// <see cref="Apply4SubApplymentStateResponse"/> 轮询（或官方通知）取得 ⇒ <b>不得把提交成功当成入驻成功</b>。
/// </para>
/// <para>
/// <b>幂等键是 <see cref="BusinessCode"/></b>：同一 <c>business_code</c> 在存在进行中的申请单时重入，
/// 官方返回 <c>400 PROCESSING</c>（见 <see cref="Apply4SubMerchantErrorCodes.Processing"/>），
/// 而非「等同一次」—— 换资料重试须先撤单或换新编号。
/// </para>
/// <para>
/// <b>敏感字段加密</b>：标注「需加密」的字段（姓名 / 证件号 / 手机号 / 邮箱 / 银行账号 / 开户名称 /
/// 证件地址）须用<b>微信支付公钥</b>（推荐）或平台证书公钥以 RSAES-OAEP 加密，
/// 并把请求头 <c>Wechatpay-Serial</c> 指向对应公钥 ID / 证书序列号（该头由传输层按配置写入）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubApplymentRequest
{
    /// <summary>业务申请编号（<c>business_code</c>，必填 string(124)）：服务商自定义唯一编号，每个编号对应一个申请单。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>超级管理员信息（<c>contact_info</c>，必填对象），见 <see cref="Apply4SubContactInfo"/>。</summary>
    [JsonPropertyName("contact_info")]
    public Apply4SubContactInfo? ContactInfo { get; set; }

    /// <summary>主体资料（<c>subject_info</c>，必填对象），见 <see cref="Apply4SubSubjectInfo"/>。</summary>
    [JsonPropertyName("subject_info")]
    public Apply4SubSubjectInfo? SubjectInfo { get; set; }

    /// <summary>经营资料（<c>business_info</c>，必填对象），见 <see cref="Apply4SubBusinessInfo"/>。</summary>
    [JsonPropertyName("business_info")]
    public Apply4SubBusinessInfo? BusinessInfo { get; set; }

    /// <summary>结算规则（<c>settlement_info</c>，必填对象），见 <see cref="Apply4SubSettlementInfo"/>。</summary>
    [JsonPropertyName("settlement_info")]
    public Apply4SubSettlementInfo? SettlementInfo { get; set; }

    /// <summary>结算银行账户（<c>bank_account_info</c>，必填对象），见 <see cref="Apply4SubBankAccountInfo"/>。</summary>
    [JsonPropertyName("bank_account_info")]
    public Apply4SubBankAccountInfo? BankAccountInfo { get; set; }

    /// <summary>补充材料（<c>addition_info</c>，选填对象），见 <see cref="Apply4SubAdditionInfo"/>。</summary>
    [JsonPropertyName("addition_info")]
    public Apply4SubAdditionInfo? AdditionInfo { get; set; }
}

/// <summary>提交进件申请单的应答（<b>仅</b> <c>applyment_id</c>）。</summary>
/// <remarks>
/// 官方本页应答字段表只有 <c>applyment_id</c>（Long）—— 旧版还回传 <c>business_code</c>，
/// 现页面已不含该字段 ⇒ 本类型<b>刻意不加</b>「大概率存在」的字段，回读业务编号请查
/// <see cref="Apply4SubApplymentStateResponse"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubApplymentResponse : WechatPayResponse
{
    /// <summary>申请单号（<c>applyment_id</c>，Long）：微信支付返回，查询申请状态的键之一。</summary>
    [JsonPropertyName("applyment_id")]
    public long? ApplymentId { get; set; }
}

/// <summary>
/// 进件申请单状态（两个查询端点<b>共用同一应答形态</b>：按 <c>applyment_id</c> 与按 <c>business_code</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：申请单号查询 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012697052"/>、
/// 业务申请编号查询 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012697168"/>
/// （2026-10-14 逐字段核验；两页应答字段表<b>逐项相同</b>，故共用一个类型而非两处复制）。
/// </para>
/// <para>
/// <b><c>sub_mchid</c> 只在 <c>APPLYMENT_STATE_FINISHED</c> 后出现</b>（官方标注选填）⇒
/// 判「进件完成」的唯一依据是 <see cref="ApplymentState"/>，不能以 <c>sub_mchid</c> 是否为空代替。
/// </para>
/// <para>
/// <b><c>sign_url</c> 是给人用的链接</b>：待账户验证 / 待签约阶段需超级管理员用微信打开，
/// SDK 不代为访问；<c>applyment_state_msg</c> 是给人看的中文说明，<b>不得</b>作分支判据（判据用 <c>applyment_state</c>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubApplymentStateResponse : WechatPayResponse
{
    /// <summary>业务申请编号（<c>business_code</c>，必填 string(124)）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>申请单号（<c>applyment_id</c>，必填 Long）。</summary>
    [JsonPropertyName("applyment_id")]
    public long? ApplymentId { get; set; }

    /// <summary>特约商户号（<c>sub_mchid</c>，选填 string(32)）：仅入驻完成后返回。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>签约 / 账户验证链接（<c>sign_url</c>，必填 string(256)）：交由超级管理员用微信打开。</summary>
    [JsonPropertyName("sign_url")]
    public string? SignUrl { get; set; }

    /// <summary>申请单状态（<c>applyment_state</c>，必填）：取值见 <see cref="Apply4SubApplymentStates"/>。</summary>
    [JsonPropertyName("applyment_state")]
    public string? ApplymentState { get; set; }

    /// <summary>状态描述（<c>applyment_state_msg</c>，必填 string(1024)）：面向人的说明文本，勿据此分支。</summary>
    [JsonPropertyName("applyment_state_msg")]
    public string? ApplymentStateMsg { get; set; }

    /// <summary>驳回详情（<c>audit_detail</c>，选填数组）：仅 <c>APPLYMENT_STATE_REJECTED</c> 时有值。</summary>
    [JsonPropertyName("audit_detail")]
    public List<Apply4SubAuditDetail>? AuditDetail { get; set; }
}

/// <summary>进件申请单驳回详情项（官方 <c>audit_detail[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubAuditDetail
{
    /// <summary>驳回字段名（<c>field</c>，必填 string(1024)）：官方给出的字段定位键，按此改资料。</summary>
    [JsonPropertyName("field")]
    public string? Field { get; set; }

    /// <summary>驳回字段中文名（<c>field_name</c>，必填 string(1024)）。</summary>
    [JsonPropertyName("field_name")]
    public string? FieldName { get; set; }

    /// <summary>驳回原因（<c>reject_reason</c>，必填 string(1024)）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }
}
