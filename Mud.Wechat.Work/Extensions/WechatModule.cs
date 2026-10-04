// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Extensions;

/// <summary>
/// 企业微信 API 模块枚举（对齐 FeishuModule）。
/// </summary>
/// <remarks>
/// 令牌签发客户端（gettoken / get_provider_token / get_suite_token / get_corp_token）
/// 位于 Abstractions 的 Authentication 注册组，随令牌底座自动注册，不经本枚举。
/// </remarks>
public enum WechatModule
{
    /// <summary>授权流接口（get_pre_auth_code / set_session_info / get_permanent_code / get_auth_info）。</summary>
    Authentication,

    /// <summary>通讯录（成员/部门/标签/通讯录查看权限/异步导入/异步导出六域）。</summary>
    Contact,

    /// <summary>客户联系（企业服务人员管理域 + 客户管理域 + 客户标签管理域 + 在职继承域：公共面 + 第三方/代开发能力差异端点）。</summary>
    ExternalContact,

    /// <summary>上下游（基础接口 + 关联客户信息 + 上下游通讯录管理：应用共享信息、下级企业凭证、小程序 session、unionid 转换、通讯录导入/查询）。</summary>
    CorpGroup,

    /// <summary>安全管理（文件防泄漏 / 设备管理 / 截屏录屏管理 / 域名 IP 信息 / 高级功能账号管理 / 操作日志，官方仅向自建应用开放）。</summary>
    Security,

    /// <summary>消息推送（发送应用消息族：三类应用公共面收敛父接口 + 第三方 template_msg 差异端点；群聊会话族 / 智能表格群聊族官方仅自建；家校学校通知族：三类应用公共面收敛父接口 + 空标记子接口）。</summary>
    Message,

    /// <summary>账号ID（ID 转换 + tmp_external_userid 转换 + 自建应用对接 + corpid 转换 + ID 迁移完成状态 + 智能机器人 userid 转换 + 群 ID 升级，跨 access/provider/suite 三种令牌路由键七接口族）。</summary>
    AccountId,

    /// <summary>微信客服（客服账号管理域 + 接待人员管理域：三类应用公共面收敛父接口 + 空标记子接口）。</summary>
    Kf,

    /// <summary>身份验证（网页授权登录/企业微信Web登录身份获取域 + 二次验证域：自建/代开发公共面收敛父接口；第三方身份获取走套件令牌独立成族）。</summary>
    Identity,

    /// <summary>企业支付（对外收款记录域：三类应用公共面收敛父接口 + 空标记子接口；收款商户号管理域 + 资金流水域 + 创建对外收款账户域 + 普通支付域 + 退款域 + 交易账单域：官方仅自建，零端点父接口 + 仅自建子接口承载端点）。</summary>
    Pay,

    /// <summary>会话内容存档（开启成员列表域 + 机器人信息域 + 会话同意情况域 + 内部群信息域：官方仅自建，零端点父接口 + 仅自建子接口承载端点；access_token 须由会话内容存档应用 secret 获取。91614 的原生 C SDK 拉取/解密面不在 HTTP SDK 范畴）。</summary>
    MsgAudit,

    /// <summary>家校沟通（家校沟通基础域：三类应用公共面收敛父接口 + 空标记子接口；家校管理配置域：官方仅自建与第三方开放，不设代开发子接口）。</summary>
    School,

    /// <summary>素材管理（临时素材上传/获取 + 上传图片 + 高清语音素材 + 异步上传临时素材：三类应用公共面收敛父接口 + 空标记子接口；服务商上传临时素材：官方仅第三方开放，走 provider_access_token 独立成族）。</summary>
    Media,
}
