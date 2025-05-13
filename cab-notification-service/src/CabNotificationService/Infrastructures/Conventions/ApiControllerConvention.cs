using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CabNotificationService.Infrastructures.Conventions
{
    public class ApiControllerConvention : IControllerModelConvention
    {
        private readonly Type _targetedControllerType;

        public ApiControllerConvention(Type targetedControllerType)
        {
            _targetedControllerType = targetedControllerType;
        }

        public void Apply(ControllerModel controller)
        {
            var type = controller.ControllerType;

            var isValidControllerType = type.BaseType?.GetGenericTypeDefinition() == _targetedControllerType &&
                                         controller.Selectors.Any(selector => selector.AttributeRouteModel != null);
            if (!isValidControllerType)
                return;

            var controllerSelectors = controller.Selectors.Where(x => x.AttributeRouteModel != null);

            foreach (var controllerSelector in controllerSelectors)
            {
                var originalTemp = controllerSelector.AttributeRouteModel?.Template;

                var newName = type.Name;
                if (newName.Contains("Admin"))
                    newName = "admin/" + type.Name.Split("Admin")[0];
                else
                    newName = type.Name.Split("Controller")[0];

                controllerSelector.AttributeRouteModel = new AttributeRouteModel
                {
                    Template = originalTemp?.Replace("[controller]", newName)
                };
            }
        }
    }
}