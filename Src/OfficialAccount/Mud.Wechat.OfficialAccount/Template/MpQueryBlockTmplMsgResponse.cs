// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Template;

/// <summary>查询拦截的模板消息（<c>wxa/sec/queryblocktmplmsg</c>）响应。</summary>
/// <remarks>
/// <para>
/// <b>官方文档矛盾（照录，逐页核验 2026-10-07）</b>：①字段表称 <c>msginfo.id</c> 类型为 string，
/// 示例 JSON 中 <c>"id": 2</c> 是数字，且请求体 <c>largest_id</c> 为 number——类型标注与示例互相矛盾
/// ⇒ SDK 按请求体 number 形态把 id 建模为 string（容纳两种形态的读取语义由调用方处理，
/// 反序列化按官方示例的宽松形态）。②接口语义为分页查询（largest_id 翻页 + limit 单页大小），
/// 但 <c>msginfo</c> 标注为单个 object 且示例只有单条记录，与「单页查询最大 100 条」语义不一致，
/// 疑似官方遗漏数组形态——SDK 照官方字段表建模为单对象。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpQueryBlockTmplMsgResponse : MpResponse
{
    /// <summary>获取或设置消息信息（官方 <c>msginfo</c>；官方文档形态矛盾照录，见类型 remarks）。</summary>
    [JsonPropertyName("msginfo")]
    public MpBlockedTmplMsgInfo? MsgInfo { get; set; }
}
