// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

// ---------------------------------------------------------------- 门店 CRUD（add_store / get_store_info / get_store_list / del_store / update_store）

/// <summary>
/// 门店图片列表（官方 <c>pic_list</c> 表单值形态 <c>{"list":[…]}</c> 的承载）。
/// </summary>
/// <remarks>
/// <para>
/// 官方原文：<c>pic_list</c> 类型为 <c>string</c>，是「一个 <b>json 字符串</b>」，官方示例结构为
/// <c>{"list":[图片url]}</c>。字段本身以字符串承载（<see cref="MpAddStoreRequest.PictureList"/> /
/// <see cref="MpUpdateStoreRequest.PictureList"/>），本 DTO 用于生成该字符串
/// （与素材域 <c>MpMaterialDescription</c> 同形先例）：
/// <c>JsonSerializer.Serialize(pics, StoreJsonContext.Default.MpStorePictureList)</c>。
/// </para>
/// <para>SDK 不做图片数量/格式拦截（官方未给出数值上限）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpStorePictureList
{
    /// <summary>获取或设置图片 URL 列表（官方 JSON 字符串内的 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<string> List { get; set; } = new List<string>();
}
