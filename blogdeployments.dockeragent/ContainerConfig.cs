namespace blogdeployments.dockeragent;

public class ContainerConfig
{
    public List<Container> Containers { get; set; }
}

public class Container
{
    public string Image { get; set; }
    public string Tag { get; set; }
    public string Name { get; set; }
    public List<Mount> Mounts { get; set; }
    public List<Port> Ports { get; set; }
}

public class Port
{
    public List<PortBinding> HostBindings { get; set; }
    public long Container { get; set; }
}

public class PortBinding
{
    public long HostPort { get; set; }
    public string HostIp { get; set; }
}

public class Mount
{
    public string Source { get; set; }
    public string Target { get; set; }
}