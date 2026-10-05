// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 通用「应用键 → 类型」注册表（对齐 <c>FeishuWebhookTypeRegistry&lt;T&gt;</c> 的分桶结构；
/// v1 方案 §5.6）：按 AppKey 隔离注册处理器/拦截器类型，注册序保留。
/// </summary>
/// <remarks>
/// <para>
/// <b>生命周期（v1.2）</b>：注册表实例在组合根期（<c>AddWechatCallback</c>）创建并以单实例注入 DI，
/// <c>AddHandler&lt;T&gt;</c>/<c>AddInterceptor&lt;T&gt;</c> 直接写入——注册只发生在宿主组装代码中
/// （单线程、先于容器构建），<b>无 Freeze 冻结语义</b>（无 Options 热更 PostConfigure 重放窗口，冻结为死代码）。
/// </para>
/// <para>
/// 通配键 <see cref="WechatCallbackOptions.WildcardAppKey"/>（<c>"*"</c>）内的条目对<b>所有</b>应用生效（D11）；
/// 内置授权族兜底处理器默认注册在该键。
/// </para>
/// </remarks>
/// <typeparam name="T">注册的标记接口类型（IWechatCallbackEventHandler / IWechatCallbackEventInterceptor）。</typeparam>
public class WechatCallbackTypeRegistry<T>
{
    private readonly ConcurrentDictionary<string, List<Type>> _registry = new();

    /// <summary>
    /// 注册类型到指定应用键桶（重复注册幂等忽略）。
    /// </summary>
    /// <param name="appKey">应用键；<see cref="WechatCallbackOptions.WildcardAppKey"/> 表示全局生效。</param>
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
    /// 按「appKey 专属 → 通配全局」顺序枚举注册类型（分发器的匹配序第一步，D11）。
    /// </summary>
    /// <param name="appKey">应用键（通配键自身不会重复枚举）。</param>
    /// <returns>专属桶在前、通配桶在后的类型序列。</returns>
    /// <remarks>
    /// 匹配序的单一权威落点：XML 回调分发器与智能机器人回调分发器共用本方法，
    /// 避免两处分发逻辑各自实现一遍桶序而漂移。
    /// </remarks>
    public IEnumerable<Type> EnumerateWithWildcard(string appKey)
    {
        foreach (var type in GetAll(appKey))
        {
            yield return type;
        }

        if (!string.Equals(appKey, WechatCallbackOptions.WildcardAppKey, StringComparison.Ordinal))
        {
            foreach (var type in GetAll(WechatCallbackOptions.WildcardAppKey))
            {
                yield return type;
            }
        }
    }
}

/// <summary>
/// 回调事件处理器注册表（键 = 应用键或通配 <see cref="WechatCallbackOptions.WildcardAppKey"/>）。
/// </summary>
public sealed class WechatCallbackHandlerRegistry : WechatCallbackTypeRegistry<IWechatCallbackEventHandler>
{
}

/// <summary>
/// 回调事件拦截器注册表（键 = 应用键或通配 <see cref="WechatCallbackOptions.WildcardAppKey"/>）。
/// </summary>
public sealed class WechatCallbackInterceptorRegistry : WechatCallbackTypeRegistry<IWechatCallbackEventInterceptor>
{
}
