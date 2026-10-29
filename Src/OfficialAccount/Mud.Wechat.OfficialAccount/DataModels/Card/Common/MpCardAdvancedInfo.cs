// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// ---------------------------------------------------------------- 跨向共用子表

/// <summary>
/// 卡券高级信息（官方 <c>advanced_info</c>）—— 建卡与查询<b>两向同表</b>，故共用一型（守卫 CD2 锁定复用关系）。
/// </summary>
/// <remarks><b>修改方向无本对象</b>：官方「修改卡券」只开放 <c>base_info</c> 与各券型标量，不含 <c>advanced_info</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardAdvancedInfo
{
    /// <summary>获取或设置使用门槛（官方 <c>use_condition</c>，选填）。</summary>
    [JsonPropertyName("use_condition")]
    public MpCardUseCondition? UseCondition { get; set; }

    /// <summary>获取或设置封面摘要（官方 <c>abstract</c>，选填；<b>官方键名为 <c>abstract</c></b>）。</summary>
    [JsonPropertyName("abstract")]
    public MpCardAbstractInfo? Abstract { get; set; }

    /// <summary>获取或设置图文列表（官方 <c>text_image_list</c>，选填）。</summary>
    [JsonPropertyName("text_image_list")]
    public List<MpCardTextImage>? TextImageList { get; set; }

    /// <summary>获取或设置商家服务类型（官方 <c>business_service</c>，选填；取值表<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("business_service")]
    public List<string>? BusinessService { get; set; }

    /// <summary>获取或设置使用时段限制（官方 <c>time_limit</c>，选填）。</summary>
    [JsonPropertyName("time_limit")]
    public List<MpCardTimeLimit>? TimeLimit { get; set; }
}
