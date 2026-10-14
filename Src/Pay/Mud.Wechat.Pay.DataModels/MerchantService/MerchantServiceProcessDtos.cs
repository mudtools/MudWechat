// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantService;

/// <summary>
/// 回复投诉单请求（官方 <c>POST /v3/merchant-service/complaints-v2/{complaint_id}/response</c> 的请求体）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467254"/>，2026-10-14 逐字段核验。应答为 204 无包体。</para>
/// <para><b>首次回复即改变状态</b>（官方原文「首次回复用户后，投诉单状态将由待处理更新为处理中」）⇒
/// 本支不是幂等的纯写入，重复调用只增加协商历史条目。</para>
/// <para><b>图片必须先传后填</b>：数组元素是「商户上传反馈图片」接口返回的 <c>media_id</c>（见
/// <see cref="MerchantServiceComplaintMedia"/> 的反查路径），官方上限 <b>4</b> 张。SDK 不校验长度，
/// 超限由官方以 <c>INVALID_REQUEST</c> 拒绝。</para>
/// <para><b><c>jump_url</c> 与 <c>jump_url_text</c> 是一对</b>（官方：「若传入跳转链接，则跳转链接文案为必传项，
/// 二者缺一不可」）且文字链属<b>灰度功能</b> ⇒ 未开通时传了也不展示，条件必填由官方拒绝，SDK 不做客户端校验。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceComplaintResponseRequest
{
    /// <summary>被诉商户号（<c>complainted_mchid</c>，必填 string(64)）：与投诉单对应，服务商代操作时填特约商户号。</summary>
    [JsonPropertyName("complainted_mchid")]
    public string? ComplaintedMchId { get; set; }

    /// <summary>回复内容（<c>response_content</c>，必填 string(500)）：具体的投诉处理方案。</summary>
    [JsonPropertyName("response_content")]
    public string? ResponseContent { get; set; }

    /// <summary>回复图片（<c>response_images</c>，选填 array[string]）：元素为上传接口返回的 <c>media_id</c>，最多 4 张。</summary>
    [JsonPropertyName("response_images")]
    public string[]? ResponseImages { get; set; }

    /// <summary>跳转链接（<c>jump_url</c>，选填 string(512)）：<b>仅支持 HTTPS</b>，灰度功能。与 <see cref="JumpUrlText"/> 必须成对。</summary>
    [JsonPropertyName("jump_url")]
    public string? JumpUrl { get; set; }

    /// <summary>跳转链接文案（<c>jump_url_text</c>，选填 string(<b>10</b>)）：展示在回复内容之后、可点击。</summary>
    [JsonPropertyName("jump_url_text")]
    public string? JumpUrlText { get; set; }

    /// <summary>跳转小程序信息（<c>mini_program_jump_info</c>，选填 object）：灰度功能，三键全必填。</summary>
    [JsonPropertyName("mini_program_jump_info")]
    public MerchantServiceComplaintMiniProgramJumpInfo? MiniProgramJumpInfo { get; set; }
}

/// <summary>
/// 回复投诉单的小程序跳转信息（官方 <c>mini_program_jump_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>与消息树里的同名对象不是一型</b>（守卫 MC-B3 锁定该分建）：本型的 <c>appid</c> / <c>path</c> / <c>text</c>
/// 官方三键<b>全为必填</b>；而 <see cref="MerchantServiceActionMiniProgramJumpInfo"/>（新版消息格式内）只有
/// <c>appid</c> / <c>path</c> 两键且皆选填、<b>没有</b> <c>text</c>。照抄合并会让必填面静默丢字段。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceComplaintMiniProgramJumpInfo
{
    /// <summary>小程序 APPID（<c>appid</c>，必填 string）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>小程序页面 PATH（<c>path</c>，必填 string）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>页面名称（<c>text</c>，必填 string）：商户自定义，展示给用户。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

/// <summary>
/// 标记投诉单处理完成请求（官方 <c>POST /v3/merchant-service/complaints-v2/{complaint_id}/complete</c> 的请求体）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467255"/>，2026-10-14 逐字段核验。应答为 204 无包体。</para>
/// <para><b>请求体只有一个字段</b>：官方正文要求「请务必在真实处理完用户投诉后再调此接口」，
/// 且「<b>重复投诉率将是考核客诉处理质量的重要指标之一</b>」⇒ 本支是对外可见的信用动作，不是收尾开关；
/// 调用后系统即对用户下发满意度评价，无法撤回。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceComplaintCompleteRequest
{
    /// <summary>被诉商户号（<c>complainted_mchid</c>，必填 string(64)）：本支唯一的请求字段。</summary>
    [JsonPropertyName("complainted_mchid")]
    public string? ComplaintedMchId { get; set; }
}

/// <summary>
/// 反馈退款审批结果请求（官方 <c>POST /v3/merchant-service/complaints-v2/{complaint_id}/update-refund-progress</c> 的请求体）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467256"/>，2026-10-14 逐字段核验。应答为 204 无包体。</para>
/// <para><b>本支请求体没有 <c>complainted_mchid</c></b>：与 <c>response</c> / <c>complete</c> 两支同族写支不同
/// （守卫 MC-B3 锁定该差异），照抄族内先例多传一个键属未定义行为。</para>
/// <para><b>「同意」不落退款，只落承诺</b>（官方原文）：<c>APPROVE</c> 后<b>必须额外调退款接口</b>发起原路退款，
/// 到账后投诉单状态才自动扭转为处理完成；<c>REJECT</c> 则当场把状态扭为处理完成。
/// ⇒ 本支只表达审批意向，退款结果不在本域闭环内。</para>
/// <para><b>条件必填互斥</b>：<c>launch_refund_day</c> 只在同意时有意义（<c>0</c> 代表当天，是合法值而非「未填」），
/// <c>reject_reason</c> / <c>reject_media_list</c> 只在拒绝时填 ⇒ 由官方 <c>INVALID_REQUEST</c> 兜，SDK 不校验。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceRefundProgressUpdateRequest
{
    /// <summary>审批动作（<c>action</c>，必填 string），取值见 <see cref="MerchantServiceRefundProgressActions"/>。</summary>
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    /// <summary>预计发起退款的工作日数（<c>launch_refund_day</c>，选填 integer）：同意退款时返回，<b>0 = 当天</b>。</summary>
    [JsonPropertyName("launch_refund_day")]
    public long? LaunchRefundDay { get; set; }

    /// <summary>拒绝退款原因（<c>reject_reason</c>，选填 string(200)）：拒绝退款时返回。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }

    /// <summary>举证图片（<c>reject_media_list</c>，选填 array[string]）：元素为上传接口返回的 <c>media_id</c>，最多 4 张。</summary>
    [JsonPropertyName("reject_media_list")]
    public string[]? RejectMediaList { get; set; }

    /// <summary>备注（<c>remark</c>，选填 string(200)）：任何需向微信支付客服反馈的信息。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}

/// <summary>
/// 回复需要即时服务的投诉单请求（官方 <c>POST /v3/merchant-service/complaints-v2/{complaint_id}/response-immediate-service</c> 的请求体）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>，2026-10-14 逐字段核验。
/// 本支是 2026 年新页（更新时间 2026.01.22），官方适用对象为列表返回的 <c>need_immediate_service=true</c> 的投诉单。</para>
/// <para><b>与 <c>response</c> 支的分工</b>：本支走新版消息格式 <see cref="MerchantServiceNormalMessage"/>
/// （富文本、图片、按钮组），<c>response</c> 支走扁平 <c>response_content</c>；两者都会「首次回复 ⇒ 状态转处理中」，
/// 不是同一支的两个入口。</para>
/// <para><b><c>idempotent_id</c> 是必填</b>（官方 string(128)，「相同 idempotent_id 的请求只会被处理一次」）⇒
/// 官方提供幂等闸，但<b>不替宿主生成</b>：留空即 <c>PARAM_ERROR</c>。重发同请求须复用同一值。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceImmediateServiceRequest
{
    /// <summary>被诉商户号（<c>complainted_mchid</c>，必填 string(64)）。</summary>
    [JsonPropertyName("complainted_mchid")]
    public string? ComplaintedMchId { get; set; }

    /// <summary>消息内容（<c>message</c>，<b>必填</b> object）：新版消息格式，块列表本身选填。</summary>
    [JsonPropertyName("message")]
    public MerchantServiceNormalMessage? Message { get; set; }

    /// <summary>幂等 ID（<c>idempotent_id</c>，必填 string(128)）：同值只被处理一次，官方不自动生成。</summary>
    [JsonPropertyName("idempotent_id")]
    public string? IdempotentId { get; set; }
}

/// <summary>
/// 回复即时服务投诉单的应答（官方 <c>log_id</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>，2026-10-14 核验。</para>
/// <para><b>本域唯一返回包体的写支</b>：其余三支（<c>response</c> / <c>complete</c> / <c>update-refund-progress</c>）
/// 均为 204 无应答包体 ⇒ 只有这一支能拿到操作流水号。官方注明该值「与查询投诉单协商历史接口返回的 <c>log_id</c> 对应」
/// ⇒ 落库它即可把「我方发出的回复」与协商历史条目对齐，用于追踪与去重。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceImmediateServiceResponse : WechatPayResponse
{
    /// <summary>操作流水号（<c>log_id</c>，必填 string(64)）：与协商历史的 <c>log_id</c> 同一命名空间。</summary>
    [JsonPropertyName("log_id")]
    public string? LogId { get; set; }
}
