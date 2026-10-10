// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Callback;

/// <summary>
/// 通用「应用键 → 类型」注册表（按 AppKey 隔离注册处理器/拦截器等类型，注册序保留）。
/// </summary>
/// <remarks>
/// <para>
/// <b>生命周期</b>：注册表实例在组合根期（各产品线的 <c>AddXxxCallback</c>）创建并以单实例注入 DI，
/// 注册只发生在宿主组装代码中（单线程、先于容器构建），<b>无 Freeze 冻结语义</b>。
/// </para>
/// <para>
/// 通配键 <see cref="WechatCallbackRouteKeys.Wildcard"/>（<c>"*"</c>）内的条目对<b>所有</b>应用生效；
/// 各产品线的内置兜底处理器默认注册在该键。
/// </para>
/// <para>
/// <b>为何落叶层</b>：本类型零产品线语义（只有分桶 + 匹配序），而「专属桶先于通配桶」是两条产品线
/// 必须一致的匹配序（漂移会让兜底处理器抢在精确处理器之前执行）。产品线以派生类承载各自的
/// 标记接口（如企微 <c>WechatCallbackHandlerRegistry</c>）。
/// </para>
/// </remarks>
/// <typeparam name="T">注册的标记接口类型（各产品线的处理器/拦截器接口）。</typeparam>
public class WechatCallbackTypeRegistry<T>
{
    private readonly ConcurrentDictionary<string, List<Type>> _registry = new();

    /// <summary>
    /// 注册类型到指定应用键桶（重复注册幂等忽略）。
    /// </summary>
    /// <param name="appKey">应用键；<see cref="WechatCallbackRouteKeys.Wildcard"/> 表示全局生效。</param>
    /// <param name="type">注册的类型。</param>
    public virtual void Register(string appKey, Type type)
    {
        if (string.IsNullOrEmpty(appKey))
        {
            throw new ArgumentException("应用键不能为空", nameof(appKey));
        }

        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        var list = _registry.GetOrAdd(appKey, _ => new List<Type>());
        lock (list)
        {
            if (!list.Contains(type))
            {
                list.Add(type);
            }
        }
    }

    /// <summary>
    /// 获取应用键桶内的全部注册类型（注册序）。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <returns>类型列表；桶不存在时为空列表。</returns>
    public virtual IReadOnlyList<Type> GetAll(string appKey)
    {
        if (!string.IsNullOrEmpty(appKey) && _registry.TryGetValue(appKey, out var list))
        {
            lock (list)
            {
                return list.ToArray();
            }
        }

        return Array.Empty<Type>();
    }

    /// <summary>
    /// 按「appKey 专属 → 通配全局」顺序枚举注册类型（分发器匹配序的第一步）。
    /// </summary>
    /// <param name="appKey">应用键（通配键自身不会重复枚举）。</param>
    /// <returns>专属桶在前、通配桶在后的类型序列。</returns>
    /// <remarks>
    /// 匹配序的单一权威落点：两条产品线的分发器共用本方法，
    /// 避免两处分发逻辑各自实现一遍桶序而漂移（守卫 CB-INV1 锁定）。
    /// </remarks>
    public IEnumerable<Type> EnumerateWithWildcard(string appKey)
    {
        foreach (var type in GetAll(appKey))
        {
            yield return type;
        }

        if (!string.Equals(appKey, WechatCallbackRouteKeys.Wildcard, StringComparison.Ordinal))
        {
            foreach (var type in GetAll(WechatCallbackRouteKeys.Wildcard))
            {
                yield return type;
            }
        }
    }
}
