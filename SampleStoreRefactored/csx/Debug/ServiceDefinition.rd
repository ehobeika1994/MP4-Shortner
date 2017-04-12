<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="SampleStoreRefactored" generation="1" functional="0" release="0" Id="1ad147b0-c423-4f34-9711-61f209f504e7" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="SampleStoreRefactoredGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="SampleStore_WebRole:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/LB:SampleStore_WebRole:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="SampleStore_WebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/MapSampleStore_WebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="SampleStore_WebRole:StorageConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/MapSampleStore_WebRole:StorageConnectionString" />
          </maps>
        </aCS>
        <aCS name="SampleStore_WebRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/MapSampleStore_WebRoleInstances" />
          </maps>
        </aCS>
        <aCS name="SampleStore_WorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/MapSampleStore_WorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="SampleStore_WorkerRole:ShortenerDbConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/MapSampleStore_WorkerRole:ShortenerDbConnectionString" />
          </maps>
        </aCS>
        <aCS name="SampleStore_WorkerRole:StorageConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/MapSampleStore_WorkerRole:StorageConnectionString" />
          </maps>
        </aCS>
        <aCS name="SampleStore_WorkerRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/MapSampleStore_WorkerRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:SampleStore_WebRole:Endpoint1">
          <toPorts>
            <inPortMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/SampleStore_WebRole/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapSampleStore_WebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/SampleStore_WebRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapSampleStore_WebRole:StorageConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/SampleStore_WebRole/StorageConnectionString" />
          </setting>
        </map>
        <map name="MapSampleStore_WebRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/SampleStore_WebRoleInstances" />
          </setting>
        </map>
        <map name="MapSampleStore_WorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/SampleStore_WorkerRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapSampleStore_WorkerRole:ShortenerDbConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/SampleStore_WorkerRole/ShortenerDbConnectionString" />
          </setting>
        </map>
        <map name="MapSampleStore_WorkerRole:StorageConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/SampleStore_WorkerRole/StorageConnectionString" />
          </setting>
        </map>
        <map name="MapSampleStore_WorkerRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/SampleStore_WorkerRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="SampleStore_WebRole" generation="1" functional="0" release="0" software="C:\Users\edmondhobeika\Desktop\SampleStoreRefactored\SampleStoreRefactored\csx\Debug\roles\SampleStore_WebRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="StorageConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;SampleStore_WebRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;SampleStore_WebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;SampleStore_WorkerRole&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/SampleStore_WebRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/SampleStore_WebRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/SampleStore_WebRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="SampleStore_WorkerRole" generation="1" functional="0" release="0" software="C:\Users\edmondhobeika\Desktop\SampleStoreRefactored\SampleStoreRefactored\csx\Debug\roles\SampleStore_WorkerRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="ShortenerDbConnectionString" defaultValue="" />
              <aCS name="StorageConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;SampleStore_WorkerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;SampleStore_WebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;SampleStore_WorkerRole&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="LocalVideoStore" defaultAmount="[2048,2048,2048]" defaultSticky="false" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/SampleStore_WorkerRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/SampleStore_WorkerRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/SampleStore_WorkerRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="SampleStore_WebRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="SampleStore_WorkerRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="SampleStore_WebRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="SampleStore_WorkerRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="SampleStore_WebRoleInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="SampleStore_WorkerRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="6623be6f-489f-4c3f-9930-dd439aaa1f39" ref="Microsoft.RedDog.Contract\ServiceContract\SampleStoreRefactoredContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="66477d42-ada2-4ca5-b79c-e4c08ef57b3e" ref="Microsoft.RedDog.Contract\Interface\SampleStore_WebRole:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/SampleStoreRefactored/SampleStoreRefactoredGroup/SampleStore_WebRole:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>