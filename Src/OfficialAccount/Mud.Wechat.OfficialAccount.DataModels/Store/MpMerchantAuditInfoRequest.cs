// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

// ---------------------------------------------------------------- 获取门店小程序审核结果（POST /wxa/get_merchant_audit_info）

/// <summary>
/// 获取门店小程序审核结果（<c>POST /wxa/get_merchant_audit_info</c>）请求体。
/// </summary>
/// <remarks>
/// <b>方法核验裁决（官方三处矛盾，勿弱化）</b>：官方「调用方式」章标 <b>GET</b>，但「请求参数」章
/// 将 <c>audit_id</c> 定义为 <b>REQUEST PAYLOAD（请求体）</b>且标必填，而请求示例为 <c>{}</c>（空对象）
/// ——三处互相矛盾。SDK 裁决为 <b>POST + 请求体</b>，依据：①「请求参数」章的<b>参数位置</b>说明（请求体）
/// 是参数归属的权威表述；② HTTP GET <b>不应携带请求体</b>（RFC 7231 语义，且生成管线不产出 GET body），
/// 若坚持 GET 则 <c>audit_id</c> 无处安放；③ 与同域其余 merchant / store 端点的形态一致。
/// 官方 GET 标注照录（方案 F8 台账）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpMerchantAuditInfoRequest
{
    /// <summary>获取或设置审核单 id（官方 <c>audit_id</c>，必填）。</summary>
    [JsonPropertyName("audit_id")]
    public int AuditId { get; set; }
}
