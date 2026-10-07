// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 回调事件处理器注册表（键 = 应用键或通配 <see cref="WechatCallbackOptions.WildcardAppKey"/>）。
/// </summary>
/// <remarks>
/// 分桶与匹配序（专属桶先于通配桶）由叶层基类
/// <c>Mud.Wechat.Abstractions.Callback.WechatCallbackTypeRegistry&lt;T&gt;</c> 承担——
/// 该匹配序是两条产品线必须一致的不变式（守卫 CB-INV1 锁定）。
/// </remarks>
public sealed class WechatCallbackHandlerRegistry : WechatCallbackTypeRegistry<IWechatCallbackEventHandler>
{
}

/// <summary>
/// 回调事件拦截器注册表（键 = 应用键或通配 <see cref="WechatCallbackOptions.WildcardAppKey"/>）。
/// </summary>
public sealed class WechatCallbackInterceptorRegistry : WechatCallbackTypeRegistry<IWechatCallbackEventInterceptor>
{
}
