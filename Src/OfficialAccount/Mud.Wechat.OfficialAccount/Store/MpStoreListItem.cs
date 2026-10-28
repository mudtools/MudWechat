// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

/// <summary>
/// 门店列表条目（官方 <c>business_list[]</c>）。
/// </summary>
/// <remarks>
/// 官方文档缺陷（照录）：返回示例出现 <c>base_info.categories</c> 与 <c>base_info.qualification_list</c>
/// （示例中均为空数组），但字段表完全没有这两个字段 ⇒ 按「存在性冲突取并集」保留。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpStoreListItem
{
    /// <summary>获取或设置门店基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpStoreBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置门店类目（官方返回示例字段 <c>base_info.categories</c>；字段表未收录，照录）。</summary>
    [JsonPropertyName("categories")]
    public List<string>? Categories { get; set; }

    /// <summary>获取或设置资质材料（官方返回示例字段 <c>base_info.qualification_list</c>；字段表未收录，照录）。</summary>
    [JsonPropertyName("qualification_list")]
    public List<string>? QualificationList { get; set; }
}
