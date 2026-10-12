// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>
/// 获取所有类目（<c>getallcategory</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 空请求体（官方变更日志：2026-09-15 HTTPS 调用由 GET 改为 POST）；
/// <c>cats</c> 为旧三级类目树、<c>cats_v2</c> 为新多级类目树，两版本嵌套结构差异见各嵌套类 XML 注释。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetAllCategoryResponse : ChannelsResponse
{
    /// <summary>获取或设置旧类目信息列表（官方 <c>cats</c>，旧三级类目树）。</summary>
    [JsonPropertyName("cats")]
    public List<ChannelsCategoryAndQua>? Cats { get; set; }

    /// <summary>获取或设置新类目信息列表（官方 <c>cats_v2</c>，新多级类目树；新旧目录树差异参阅官方「新旧类目差异说明」）。</summary>
    [JsonPropertyName("cats_v2")]
    public List<ChannelsCategoryAndQua>? CatsV2 { get; set; }
}
