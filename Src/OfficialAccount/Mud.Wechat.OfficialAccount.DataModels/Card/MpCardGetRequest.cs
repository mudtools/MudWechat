// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// ---------------------------------------------------------------- 查询卡券（POST /card/get）
//  应答为「card 包装 + card_type 判别 + 券型分支」，与建卡方向**分支集不同**（本向无 general_card），
//  且 base_info 为查询形态（含 id/status/时间戳）。分支之间互斥 ⇒ 各支独立对象，不得合并成万能对象。

/// <summary>
/// 查询卡券详情（<c>POST /card/get</c>）请求体。
/// </summary>
/// <remarks>官方请求体仅一个字段 <c>card_id</c>；<c>access_token</c> 走 Query、不入请求体。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGetRequest
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;
}
