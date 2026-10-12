// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 获取卡券列表（<c>POST /card/batchget</c>）应答。
/// </summary>
/// <remarks>
/// <b>字段名为 <c>card_id_list</c> / <c>total_num</c></b>（对齐基准：SKIT <c>CardBatchGetResponse</c>）——
/// <b>不是</b> <c>card_list</c> / <c>total_count</c>；SDK 不凭常识改写，守卫 CD6 锁定。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardBatchGetResponse : MpResponse
{
    /// <summary>获取或设置本页卡券模板编号列表（官方 <c>card_id_list</c>）。</summary>
    [JsonPropertyName("card_id_list")]
    public List<string>? CardIdList { get; set; }

    /// <summary>获取或设置卡券总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int TotalNum { get; set; }
}
