// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NETSTANDARD2_0 || NET6_0

using System.ComponentModel;

// C# 9 的 init 访问器与 C# 11 的 required 成员依赖「编译器要求的成员」，BCL 分别在 net5.0 / net7.0 才提供。
// 本文件为低两档补齐这些标记类型（标准做法，仅类型占位、无运行时行为），使审计/审批模型在四档保持同一公开面。
namespace System.Runtime.CompilerServices
{
    /// <summary>init 访问器的编译器标记类型占位。</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }

    /// <summary>required 成员的编译器标记类型占位。</summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute
    {
    }

    /// <summary>编译器特性要求标记占位。</summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        /// <summary>初始化标记。</summary>
        /// <param name="featureName">特性名（编译器写入）。</param>
        public CompilerFeatureRequiredAttribute(string featureName) => FeatureName = featureName;

        /// <summary>特性名。</summary>
        public string FeatureName { get; }

        /// <summary>是否可选。</summary>
        public bool IsOptional { get; set; }
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>标注构造函数已为 required 成员赋值的标记占位。</summary>
    [AttributeUsage(AttributeTargets.Constructor, Inherited = false)]
    internal sealed class SetsRequiredMembersAttribute : Attribute
    {
    }
}

#endif
