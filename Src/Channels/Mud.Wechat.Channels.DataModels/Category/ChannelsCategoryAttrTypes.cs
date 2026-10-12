// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>属性类型 v2 常量（官方 <c>type_v2</c> 枚举值，7 种）。</summary>
public static class ChannelsCategoryAttrTypes
{
    /// <summary>文本（自定义输入）。</summary>
    public const string String = "string";

    /// <summary>多选一。</summary>
    public const string SelectOne = "select_one";

    /// <summary>多选。</summary>
    public const string SelectMany = "select_many";

    /// <summary>整数。</summary>
    public const string Integer = "integer";

    /// <summary>4 位精度小数。</summary>
    public const string Decimal4 = "decimal4";

    /// <summary>整数 + 单位。</summary>
    public const string IntegerUnit = "integer_unit";

    /// <summary>4 位精度小数 + 单位。</summary>
    public const string Decimal4Unit = "decimal4_unit";
}
