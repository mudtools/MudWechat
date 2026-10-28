// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.QrcodeJump;

/// <summary>
/// 获取已设置的二维码规则（<c>POST /cgi-bin/wxopen/qrcodejumpget</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>rule_list</c>（二维码规则详情列表）/
/// <c>qrcodejump_open</c>（是否已打开二维码跳转链接设置）/ <c>list_size</c>（二维码规则数量）/
/// <c>qrcodejump_pub_quota</c>（<b>本月还可发布的次数</b>）/ <c>total_count</c>（规则总数据量，用于分页查询）。
/// </para>
/// <para>
/// <b>官方文档缺陷（照录）</b>：返回示例中出现 <c>permit_sub_rule</c> 字段，但返回参数表未收录、未定义
/// ——SDK 不建模该字段（示例字段无字段表形态说明）。
/// </para>
/// <para>官方错误码：<c>-1</c> / <c>0</c> / <c>40001</c> / <c>40097</c> / <c>40166</c> / <c>44990</c> / <c>85075</c> / <c>886001</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrcodeJump")]
public class MpQrcodeJumpGetResponse : MpResponse
{
    /// <summary>获取或设置二维码规则详情列表（官方 <c>rule_list</c>）。</summary>
    [JsonPropertyName("rule_list")]
    public List<MpQrcodeJumpRule>? RuleList { get; set; }

    /// <summary>获取或设置是否已打开二维码跳转链接设置（官方 <c>qrcodejump_open</c>）。</summary>
    [JsonPropertyName("qrcodejump_open")]
    public int? QrcodeJumpOpen { get; set; }

    /// <summary>获取或设置二维码规则数量（官方 <c>list_size</c>）。</summary>
    [JsonPropertyName("list_size")]
    public int? ListSize { get; set; }

    /// <summary>获取或设置本月还可发布的次数（官方 <c>qrcodejump_pub_quota</c>）。</summary>
    [JsonPropertyName("qrcodejump_pub_quota")]
    public int? QrcodeJumpPublishQuota { get; set; }

    /// <summary>获取或设置二维码规则总数据量（官方 <c>total_count</c>，用于分页查询）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }
}
