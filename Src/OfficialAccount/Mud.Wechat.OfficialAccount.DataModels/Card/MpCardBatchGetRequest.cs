// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// ---------------------------------------------------------------- 获取卡券列表（POST /card/batchget）

/// <summary>
/// 获取卡券列表（<c>POST /card/batchget</c>）请求体。
/// </summary>
/// <remarks>
/// <b>分页形态</b>：官方为 <c>offset</c> + <c>count</c>（<b>无游标、无 page_info</b>），
/// 与素材域 <c>batchget</c> 的游标形态不同；<c>status_list</c> 为状态筛选数组（官方状态枚举<b>待逐页核验</b>，
/// SDK 取 <c>string</c> 不建枚举常量）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardBatchGetRequest
{
    /// <summary>获取或设置分页偏移量（官方 <c>offset</c>，必填，从 0 开始）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置单页取数上限（官方 <c>count</c>，必填；上限数值<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>获取或设置卡券状态筛选（官方 <c>status_list</c>，选填；取值表<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("status_list")]
    public List<string>? StatusList { get; set; }
}
