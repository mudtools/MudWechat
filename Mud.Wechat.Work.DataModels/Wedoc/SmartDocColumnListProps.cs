// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 分栏内容块属性（官方 ColumnListProps，对应 <c>BLOCK_TYPE_COLUMN_LIST</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartDocColumnListProps
{
    /// <summary>
    /// 获取或设置分栏数量（官方 <c>column_num</c>）。
    /// <para>官方业务限制：有效范围为 <c>[2, 4]</c>；小于 <c>2</c> 按 <c>2</c> 处理、大于 <c>4</c> 按 <c>4</c> 处理，
    /// 缺省或传入 <c>0</c> 时按最小值 <c>2</c> 处理。</para>
    /// </summary>
    [JsonPropertyName("column_num")]
    public uint? ColumnNum { get; set; }
}
