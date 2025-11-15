namespace NRayUI.Components

type ConsoleModel = {
    Input: string
    Output: Result<string, string> list
    History: string list
}
