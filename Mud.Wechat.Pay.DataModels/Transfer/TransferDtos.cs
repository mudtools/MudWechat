// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Transfer;

/// <summary>
/// 商家转账·发起转账（<c>POST /v3/fund-app/mch-transfer/transfer-bills</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716434"/>
/// （2026-10-09 逐字段核验；更新时间 2025.03.21）。<b>支持商户：【普通商户】</b>；
/// 官方注明单个商户接口频率限制 <b>100 次/s</b>。
/// </para>
/// <para>
/// <b>🔴 资金安全（官方原文，务必遵守）</b>：官方明示「如果发起转账接口遇到新的错误码，
/// 请务必<b>不要换单重试</b>，需通过『商户单号查询转账单』或『微信单号查询转账单』API 查询订单结果，
/// <b>当查询原订单结果明确为「失败」时</b>，再更换商户订单号进行重试。否则会有<b>重复转账的资金风险</b>。」
/// —— 即：<b>换 <c>out_bill_no</c> 重试前必须先查清原单状态</b>，这是本域区别于其它域的第一原则。
/// </para>
/// <para>
/// <b>⚠️ 值域未核验（诚实记录）</b>：本轮核验覆盖<b>字段名与必填性</b>，未取得各枚举字段的<b>取值表</b>
/// —— <c>state</c>、<c>transfer_scene_id</c>、<c>user_recv_perception</c>、<c>user_recv_style.type</c>
/// 的值域均<b>未</b>核实 ⇒ SDK <b>不臆造</b>常量，一律保持字符串，留待后续增量补值表与守卫。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferBillRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户转账单号（<c>out_bill_no</c>，必填 string(32)）：<b>业务幂等键</b>（见类型 remarks 的换单重试红线）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>转账场景 ID（<c>transfer_scene_id</c>，必填 string(36)）：值域未核验（见类型 remarks）。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }

    /// <summary>收款用户 OpenID（<c>openid</c>，必填 string(64)）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>收款用户姓名（<c>user_name</c>，选填）：官方标注为需加密字段（按官方加密指引处理）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>转账金额（<c>transfer_amount</c>，必填 integer，单位分）。</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>转账备注（<c>transfer_remark</c>，必填 string(32)）：用户可见文案。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，选填 string(256)）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>用户收款感知（<c>user_recv_perception</c>，选填）：值域未核验（见类型 remarks）。</summary>
    [JsonPropertyName("user_recv_perception")]
    public string? UserRecvPerception { get; set; }

    /// <summary>转账场景报备信息（<c>transfer_scene_report_infos</c>，<b>必填</b>），见 <see cref="TransferSceneReportInfo"/>。</summary>
    [JsonPropertyName("transfer_scene_report_infos")]
    public List<TransferSceneReportInfo>? TransferSceneReportInfos { get; set; }

    /// <summary>用户收款样式（<c>user_recv_style</c>，选填），见 <see cref="TransferUserRecvStyle"/>。</summary>
    [JsonPropertyName("user_recv_style")]
    public TransferUserRecvStyle? UserRecvStyle { get; set; }
}

/// <summary>转账场景报备信息（<c>transfer_scene_report_infos[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferSceneReportInfo
{
    /// <summary>报备信息类型（<c>info_type</c>，必填 string(15)）：值域未核验。</summary>
    [JsonPropertyName("info_type")]
    public string? InfoType { get; set; }

    /// <summary>报备信息内容（<c>info_content</c>，必填 string(32)）。</summary>
    [JsonPropertyName("info_content")]
    public string? InfoContent { get; set; }
}

/// <summary>用户收款样式（<c>user_recv_style</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferUserRecvStyle
{
    /// <summary>样式类型（<c>type</c>，必填 string）：值域未核验（见 <see cref="TransferBillRequest"/> 的 remarks）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

/// <summary>
/// 发起转账应答（5 个平铺字段）。
/// </summary>
/// <remarks>
/// <b>注意</b>：<c>package_info</c> 是<b>调起用户确认收款</b>所需的凭据（官方另有 APP/JSAPI
/// 「调起用户确认收款」两份客户端文档）—— 它<b>不</b>代表转账已成功，<c>state</c> 才是状态位
/// （其值域本轮未核验）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferBillResponse : WechatPayResponse
{
    /// <summary>商户转账单号（<c>out_bill_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>微信转账单号（<c>transfer_bill_no</c>，必填 string(64)）：微信系统返回的唯一标识。</summary>
    [JsonPropertyName("transfer_bill_no")]
    public string? TransferBillNo { get; set; }

    /// <summary>转账单创建时间（<c>create_time</c>，必填 string）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>转账单状态（<c>state</c>，必填 string）：<b>值域未核验</b>，勿臆造取值判定。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>调起用户确认收款凭据（<c>package_info</c>，选填 string）：<b>不等于</b>转账成功。</summary>
    [JsonPropertyName("package_info")]
    public string? PackageInfo { get; set; }
}

/// <summary>
/// 商户单号查询转账单应答（<c>GET /v3/fund-app/mch-transfer/transfer-bills/out-bill-no/{out_bill_no}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716437"/>
/// （2026-10-09 逐字段核验；更新时间 2025.03.21）。<b>支持商户：【普通商户】</b>。
/// </para>
/// <para>
/// <b>本端点是「不换单重试」红线（见 <see cref="TransferBillRequest"/> remarks）的执行手段</b>：
/// 发起转账遇错后<b>必须</b>先据此查单；<c>state</c> 取 <c>ACCEPTED</c> / <c>PROCESSING</c> 时官方明确
/// <b>应「原单重试」</b>（<b>不要</b>换 <c>out_bill_no</c>）—— 语义见 <see cref="TransferBillStates"/>。
/// </para>
/// <para>
/// <b>⚠️ 字段名陷阱</b>：官方本页商户号字段是 <c>mch_id</c>（<b>带下划线</b>），
/// 而支付线其它域（交易 / 退款 / 分账 / 支付分…）一律是 <c>mchid</c>（<b>无下划线</b>）。
/// 二者<b>不可互相「纠正」</b> —— 这正是守卫要拦的「顺手统一」类漂移。另注意本页参数表
/// <b>未</b>列出嵌套对象（<c>transfer_scene_id</c> / <c>user_recv_perception</c> 也不在本页）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferBillQueryResponse : WechatPayResponse
{
    /// <summary>商户号（<c>mch_id</c>，<b>带下划线</b> —— 与支付线其它域的 <c>mchid</c> 不同）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>商户转账单号（<c>out_bill_no</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>微信转账单号（<c>transfer_bill_no</c>，string(64)）。</summary>
    [JsonPropertyName("transfer_bill_no")]
    public string? TransferBillNo { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>转账单状态（<c>state</c>）：取值与重试语义见 <see cref="TransferBillStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>转账金额（<c>transfer_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>转账备注（<c>transfer_remark</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>失败原因（<c>fail_reason</c>）：<c>state = FAIL</c> 时的原因说明。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>收款用户 OpenID（<c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>收款用户姓名（<c>user_name</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>转账单创建时间（<c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>转账单更新时间（<c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }
}

/// <summary>
/// 商户单号查询电子回单应答（<c>GET /v3/fund-app/mch-transfer/elecsign/out-bill-no/{out_bill_no}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716436"/>
/// （2026-10-09 逐字段核验；更新时间 2025.03.21）。<b>支持商户：【普通商户】</b>。
/// </para>
/// <para>
/// <b>🔴 <c>download_url</c> 有效期仅 <b>10 分钟</b></b>（官方原文），过期须重新调用本接口获取；
/// 且官方明示「<b>域名/路径/参数都可能变化，请勿自行拼接</b>」⇒ 必须原样使用返回的 URL。
/// </para>
/// <para>
/// <b>⚠️ 下载安全提示</b>：本仓的账单下载通道（<c>IWechatPayBillDownloadService</c>）带<b>主机白名单前置闸</b>。
/// 回单域名若不在该白名单内会被<b>拒绝</b>（fail-closed，正确行为）—— 宿主若需下载回单，
/// 须先核验回单域名并自行放行，<b>不得</b>为了「能下」而放宽白名单。
/// </para>
/// <para>
/// <b>完整性校验</b>：<c>hash_type</c> / <c>hash_value</c> 仅在 <c>state = FINISHED</c> 时返回，
/// 官方要求<b>与下载到的文件摘要比对</b>以确认完整性与真实性 —— 不要只看下载成功就采信。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferElecsignResponse : WechatPayResponse
{
    /// <summary>申请单状态（<c>state</c>，必填）：取值见 <see cref="TransferElecsignStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>申请单创建时间（<c>create_time</c>，必填，rfc3339）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>申请单更新时间（<c>update_time</c>，必填，rfc3339）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>回单文件摘要类型（<c>hash_type</c>，选填）：<b>仅</b>申请单已完成时返回，见 <see cref="TransferElecsignHashTypes"/>。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>回单文件摘要值（<c>hash_value</c>，选填）：<b>仅</b>已完成时返回，须与下载文件摘要比对。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

    /// <summary>
    /// 回单文件下载地址（<c>download_url</c>，选填）：<b>仅</b>已完成时返回，
    /// <b>有效期 10 分钟</b>，且官方要求<b>原样使用、勿自行拼接</b>。
    /// </summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }
}

/// <summary>
/// 撤销转账应答（<c>POST /v3/fund-app/mch-transfer/transfer-bills/out-bill-no/{out_bill_no}/cancel</c>，<b>仅 4 字段</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716458"/>
/// （2026-10-09 逐字段核验；更新时间 2025.03.18）。<b>支持商户：【普通商户】</b>。
/// </para>
/// <para>
/// <b>⚠️ 本接口<b>无请求体</b></b>（官方字段表明确：仅 path 参数 <c>out_bill_no</c>，不携带请求体字段）
/// ⇒ 接口方法<b>只有</b> path 参数，刻意不造「空请求体」DTO（那会向官方发送一个它没定义的 <c>{}</c>）。
/// </para>
/// <para>
/// <b>为何不复用 <see cref="TransferBillQueryResponse"/></b>：官方本页应答<b>只有</b>
/// <c>out_bill_no</c> / <c>transfer_bill_no</c> / <c>state</c> / <c>update_time</c> 四项，
/// <b>无</b> <c>mch_id</c> / <c>appid</c> / 金额 / 收款人等信息 ⇒ 与查询应答不是同一张表，独立建模。
/// </para>
/// <para>
/// <b>异步语义（官方原文）</b>：返回成功<b>仅表示撤销请求已受理</b>，系统会异步处理退款等操作，
/// <b>以最终查询单据返回状态为准</b>；且仅在<b>用户确认收款之前</b>可撤销。
/// 本页 <c>state</c> 只出现 <c>CANCELING</c>（撤销中）与 <c>CANCELLED</c>（已撤销）
/// —— 取值仍属 <see cref="TransferBillStates"/> 那一套 <c>state</c> 枚举的子集。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferRevokeResponse : WechatPayResponse
{
    /// <summary>商户单号（<c>out_bill_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>微信转账单号（<c>transfer_bill_no</c>，必填 string(64)）：商家转账订单的主键。</summary>
    [JsonPropertyName("transfer_bill_no")]
    public string? TransferBillNo { get; set; }

    /// <summary>单据状态（<c>state</c>，必填）：撤销场景为 <c>CANCELING</c> / <c>CANCELLED</c>，见 <see cref="TransferBillStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>最后一次单据状态变更时间（<c>update_time</c>，必填，rfc3339）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }
}
