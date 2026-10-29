// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Auth;

/// <summary>
/// 个人认证身份选项树节点（官方 <c>identity_tree_list</c> 的元素，<b>递归自指</b>：子级同为 <c>node_list</c>，元素与本节点同形状）。
/// 非叶节点带 <c>node_list</c>，叶节点带 <c>leaf_info</c>；根节点带 <c>root_info</c>（三者在同一节点上至多其一出现）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_authidentitytree.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class OpenPlatformAuthIdentityTreeNode
{
    /// <summary>节点名称（官方 <c>name</c>，string）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>节点 ID（官方 <c>node_id</c>，number）。</summary>
    [JsonPropertyName("node_id")]
    public long NodeId { get; set; }

    /// <summary>根节点信息（官方 <c>root_info</c>，object；仅根节点出现）。</summary>
    [JsonPropertyName("root_info")]
    public OpenPlatformAuthIdentityTreeRootInfo? RootInfo { get; set; }

    /// <summary>叶节点信息（官方 <c>leaf_info</c>，object；仅叶子出现）。</summary>
    [JsonPropertyName("leaf_info")]
    public OpenPlatformAuthIdentityTreeLeafInfo? LeafInfo { get; set; }

    /// <summary>子节点列表（官方 <c>node_list</c>，递归数组，元素与本节点同形状；仅非叶节点出现）。</summary>
    [JsonPropertyName("node_list")]
    public List<OpenPlatformAuthIdentityTreeNode>? NodeList { get; set; }
}
