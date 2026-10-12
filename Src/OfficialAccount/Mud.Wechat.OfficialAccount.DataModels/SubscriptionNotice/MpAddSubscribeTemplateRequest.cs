// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SubscriptionNotice;

/// <summary>
/// 选用模板（<c>wxaapi/newtmpl/addtemplate</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>tid</c>（模板标题 id）/<c>kidList</c>/<c>sceneDesc</c> 均必填。
/// 官方「注意事项」原文：①「模板标题 id 可通过接口获取或后台查看」；②「<b>关键词组合需 2-5 个</b>」；
/// ③「<b>服务场景描述限制 15 字</b>」。
/// </para>
/// <para><c>kidList</c> 为关键词 id 数字列表，顺序可自由搭配（官方示例 [3,5,4]）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpAddSubscribeTemplateRequest
{
    /// <summary>获取或设置模板标题 id（官方 <c>tid</c>，必填；官方示例为字符串形态 "401"）。</summary>
    [JsonPropertyName("tid")]
    public string Tid { get; set; } = string.Empty;

    /// <summary>获取或设置组合好的模板关键词 id 列表（官方 <c>kidList</c>，数字数组；2~5 个，顺序可自由搭配）。</summary>
    [JsonPropertyName("kidList")]
    public List<int> KidList { get; set; } = new List<int>();

    /// <summary>获取或设置服务场景描述（官方 <c>sceneDesc</c>，15 个字以内）。</summary>
    [JsonPropertyName("sceneDesc")]
    public string SceneDesc { get; set; } = string.Empty;
}
