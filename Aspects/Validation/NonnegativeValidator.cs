﻿using System;
using System.Diagnostics.Contracts;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using vm.Aspects.Properties;

namespace vm.Aspects.Validation
{
    /// <summary>
    /// Tests if the target element is a number equal to or greater than zero, i.e. that it is non-negative.
    /// </summary>
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public sealed class NonnegativeValidator : ValueValidator
    {
        readonly IComparable _zero;

        /// <summary>
        /// Initializes the validator.
        /// </summary>
        public NonnegativeValidator(
            IComparable zero,
            string messageTemplate,
            string tag,
            bool negated)
            : base(messageTemplate, tag, negated)
        {
            Contract.Requires<ArgumentNullException>(zero != null, nameof(zero));

            _zero = zero;
        }

        /// <summary>
        /// Implements the validation logic for the receiver.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate" />.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        /// <remarks>Subclasses must provide a concrete implementation the validation logic.</remarks>
        public override void DoValidate(
            object objectToValidate,
            object currentTarget,
            string key,
            ValidationResults validationResults)
        {
            var comparable = objectToValidate as IComparable;
            var valid = comparable != null &&
                        comparable.CompareTo(_zero) >= 0;

            if (Negated)
                valid = !valid;

            if (!valid)
                LogValidationResult(validationResults, GetMessage(objectToValidate, key), currentTarget, key);
        }

        /// <summary>
        /// Gets the default message template if the validation fails.
        /// </summary>
        protected override string DefaultNegatedMessageTemplate => Resources.ExNegatedNonnegativeValidation;

        /// <summary>
        /// Gets the default message template if the validation fails.
        /// </summary>
        protected override string DefaultNonNegatedMessageTemplate => Resources.ExNonnegativeValidation;
    }
}
