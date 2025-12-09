using System.Collections;

public class MenuValueEditor
{
    public int InitialValue { get; set; }
    public int StepUp { get; set; }
    public int StepDown { get; set; }

    public MenuValueEditor(int initialValue, int stepUp, int stepDown)
    {
        InitialValue = initialValue;
        StepUp = stepUp;
        StepDown = stepDown;
    }
}

public class MenuItemHandler
{
    public string LcdLine1Message { get; set; }
    public MenuValueEditor ValueEditor { get; set; }

    public MenuItemHandler(string lcdLine1Message, MenuValueEditor valueEditor)
    {
        LcdLine1Message = lcdLine1Message;
        ValueEditor = valueEditor;
    }
}

public class MenuItem
{
    public string Name { get; set; }
    public string MenuEntry { get; set; }
    public MenuItemHandler Handler { get; set; }
    public Hashtable SubMenus { get; set; }

    public MenuItem(string name, string menuEntry, MenuItemHandler handler)
    {
        Name = name;
        MenuEntry = menuEntry;
        Handler = handler;
        SubMenus = new Hashtable();
    }
}

// This class manages the menus and holds the hashtable of all menus.
public class MenuDefinition
{
    public Hashtable Menus { get; set; }

    public MenuDefinition()
    {
        Menus = new Hashtable();
    }
}
