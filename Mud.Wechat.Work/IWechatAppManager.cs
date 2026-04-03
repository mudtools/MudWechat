using Mud.Wechat.Work.Options;

namespace Mud.Wechat.Work;

/// <summary>
/// 微信应用管理器。
/// </summary>
public interface IWechatAppManager : IAppManager<IWechatAppContext>
{
    /// <summary>
    /// 运行时添加应用
    /// </summary>
    /// <param name="config">应用配置</param>
    /// <returns>新创建的应用上下文</returns>
    /// <exception cref="InvalidOperationException">当应用已存在或配置无效时抛出</exception>
    /// <remarks>
    /// 在运行时动态添加一个新的飞书应用。
    /// 应用配置会自动验证，验证通过后会创建对应的应用上下文。
    /// 如果应用键已存在，会抛出异常。
    /// </remarks>
    IWechatAppContext AddApp(WechatWorkClientOptions config);

    /// <summary>
    /// 默认的应用配置
    /// </summary>
    /// <remarks>
    /// 包含此应用的所有配置信息，如AppId、AppSecret、BaseUrl等。
    /// </remarks>
    WechatWorkClientOptions DefaultConfig { get; }
}
