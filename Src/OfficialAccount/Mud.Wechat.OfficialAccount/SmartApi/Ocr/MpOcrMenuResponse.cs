// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

/// <summary>
/// 菜单识别（<c>POST /cv/ocr/menu</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>content</c>（识别的信息）为 object，内含
/// <c>menu_items</c>（菜单内容列表，条目字段 <c>name</c> 菜单名 / <c>price</c> 价格）。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录，SDK 以字段表为口径）</b>：官方返回示例把 <c>content</c> 写成 JSON <b>字符串</b>
/// （需二次解析），与字段表标注的 object 不一致；SDK 按字段表建模为对象
/// （与素材域「字段表与正文不一致时以字段表为口径」的既存裁决同源）。若实测为字符串，宿主需自行二次解析。
/// </para>
/// <para>
/// <b>本页无频率上限</b>（官方全页无频次章节，且原文「该接口尚未接入服务平台，暂时不支持付费购买」）
/// ——SDK 不编造数值。错误码：<c>-1</c> / <c>101000</c> / <c>101001</c> / <c>101002</c>
/// （本页错误码表未列 <c>40001</c>，照录）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrMenuResponse : MpResponse
{
    /// <summary>获取或设置识别的信息（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public MpOcrMenuContent? Content { get; set; }
}
