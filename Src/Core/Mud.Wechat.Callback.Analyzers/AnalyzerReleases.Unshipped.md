; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
MUDCB002 | MudWechatCallback | Error | 处理器事件键与载荷契约不一致
MUDCB003 | MudWechatCallback | Info | 处理器事件键不是编译期常量
MUDCB004 | MudWechatCallback | Warning | 处理器事件键应引用 WechatCallbackEventTypes 常量
MUDCB005 | MudWechatCallback | Warning | 载荷处理器未注册